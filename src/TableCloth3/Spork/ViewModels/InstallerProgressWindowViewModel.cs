using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotNext.Threading;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Channels;
using System.Windows.Input;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerProgressWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindowViewModel(
        IMessenger messenger)
        : this()
    {
        _messenger = messenger;
    }

    public InstallerProgressWindowViewModel()
        : base()
    {
    }

    private readonly IMessenger _messenger = default!;

    public sealed record class CancelNotification(bool dueToError, Exception? foundException);

    public interface ICancelNotificationRecipient : IRecipient<CancelNotification>;

    public sealed record class FinishNotification;

    public interface IFinishNotificationRecipient : IRecipient<FinishNotification>;

    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            Steps.Add(new()
            {
                StepProgress = (StepProgress)(i % Enum.GetValues<StepProgress>().Count()),
                PackageName = $"Item {i+1}",
                PackageUrl = "https://yourtablecloth.app/",
                PackageArguments = "/S",
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken = default)
        => await RunInstallerStepsAsync(cancellationToken).ConfigureAwait(false);

    [RelayCommand]
    private void CancelButton()
        => _messenger.Send<CancelNotification>(new(false, default));

    private async Task RunInstallerStepsAsync(CancellationToken cancellationToken = default)
    {
        var loadQueue = new Queue<InstallerStepItemViewModel>();
        var installQueue = new Queue<InstallerStepItemViewModel>();

        try
        {
            foreach (var eachStep in Steps)
                loadQueue.Enqueue(eachStep);

            var loadTask = Task.Run(async () =>
            {
                while (loadQueue.Count > 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    var item = loadQueue.Dequeue();
                    try
                    {
                        item.StepProgress = StepProgress.Loading;
                        await item.LoadInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                        installQueue.Enqueue(item);
                        item.Event.Set();
                        item.StepProgress = StepProgress.Ready;
                    }
                    catch (Exception ex)
                    {
                        item.StepError = ex.Message;
                        item.StepProgress = StepProgress.Failed;
                        _messenger.Send<CancelNotification>(new(true, ex));
                    }
                }
            }, cancellationToken);

            var installTask = Task.Run(async () =>
            {
                var metEndOfSuite = false;

                while (!metEndOfSuite)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (!installQueue.TryDequeue(out var item))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5d), cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    using (item)
                    {
                        await item.Event.WaitAsync(cancellationToken).ConfigureAwait(false);

                        if (item.ItemType == ItemType.EndOfSuite)
                        {
                            metEndOfSuite = true;
                            continue;
                        }
                        else
                        {
                            try
                            {
                                item.StepProgress = StepProgress.Installing;
                                await item.PerformInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                                item.StepProgress = StepProgress.Succeed;
                            }
                            catch (Exception ex)
                            {
                                item.StepError = ex.Message;
                                item.StepProgress = StepProgress.Failed;
                                _messenger.Send<CancelNotification>(new(true, ex));
                            }
                        }
                    }
                }
            }, cancellationToken);

            await Task.WhenAll(loadTask, installTask).ConfigureAwait(false);
            _messenger.Send<FinishNotification>();
        }
        catch (Exception ex)
        {
            _messenger.Send<CancelNotification>(new(true, ex));
        }
    }
}

public sealed partial class InstallerStepItemViewModel : BaseViewModel, IDisposable, IAsyncDisposable
{
    [ObservableProperty]
    private ItemType _itemType = ItemType.None;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private string _packageName = string.Empty;

    [ObservableProperty]
    private string _packageUrl = string.Empty;

    [ObservableProperty]
    private string _packageArguments = string.Empty;

    [ObservableProperty]
    private string _stepError = string.Empty;

    [ObservableProperty]
    private StepProgress _stepProgress = StepProgress.None;

    internal AsyncManualResetEvent Event => _mre;

    private AsyncManualResetEvent _mre = new AsyncManualResetEvent(false);
    private bool _disposedValue;

    partial void OnStepErrorChanged(string value)
        => OnPropertyChanged(nameof(HasError));

    partial void OnStepProgressChanged(StepProgress value)
        => OnPropertyChanged(nameof(StatusText));

    [RelayCommand]
    private async Task LoadInstallStep(CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(3d), cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private async Task PerformInstallStep(CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(2d), cancellationToken).ConfigureAwait(false);
    }

    public string StatusText => StepProgress switch
    {
        StepProgress.Loading => "\u23f3",
        StepProgress.Ready => "\ud83d\udce6",
        StepProgress.Installing => "\ud83d\udee0\ufe0f",
        StepProgress.Succeed => "\u2714\ufe0f",
        StepProgress.Failed => "\u274c",
        StepProgress.Unknown => "\u2754",
        _ => "\u2b1c",
    };

    public bool HasError => !string.IsNullOrWhiteSpace(StepError);

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _mre.Dispose();
            }

            _mre = null!;
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_mre != null)
        {
            await _mre.DisposeAsync().ConfigureAwait(false);
            _mre = null!;
        }

        Dispose(false);
        GC.SuppressFinalize(this);
    }
}

public enum StepProgress
{
    None,
    Loading,
    Ready,
    Installing,
    Succeed,
    Failed,
    Unknown,
}

public enum ItemType
{
    None,
    EndOfSuite,
    InstallerBinary,
    PowerShellScript,
}