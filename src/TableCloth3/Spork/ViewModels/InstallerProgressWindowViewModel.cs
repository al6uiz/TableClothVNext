using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Shared.ViewModels;
using TableCloth3.Spork.Contracts;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerProgressWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindowViewModel(
        ITaskBarProgressService taskBarProgressService,
        IMessenger messenger)
        : this()
    {
        TaskBarProgressService = taskBarProgressService;
        _messenger = messenger;
    }

    public InstallerProgressWindowViewModel()
        : base()
    {
    }

    private readonly IMessenger _messenger = default!;
    public ITaskBarProgressService TaskBarProgressService { get; } = null!;

    public sealed record class CancelNotification(bool dueToError, Exception? foundException);

    public interface ICancelNotificationRecipient : IRecipient<CancelNotification>;

    public sealed record class FinishNotification;

    public interface IFinishNotificationRecipient : IRecipient<FinishNotification>;

    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            var progress = (StepProgress)(i % Enum.GetValues<StepProgress>().Count());
            Steps.Add(new()
            {
                StepProgress = progress,
                PackageName = $"Item {i + 1}",
                PackageUrl = "https://yourtablecloth.app/",
                PackageArguments = "/S",
                StepError = progress == StepProgress.Failed ? "An error occurred while processing this step." : string.Empty,
                Percentage = Random.Shared.Next(0, 100),
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken = default)
    {
        if (Design.IsDesignMode)
            return;

        await RunInstallerStepsAsync(cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private void CancelButton()
        => _messenger.Send<CancelNotification>(new(false, default));

    private async Task RunInstallerStepsAsync(CancellationToken cancellationToken = default)
    {
        var loadQueue = new Queue<InstallerStepItemViewModel>();
        var installQueue = new Queue<InstallerStepItemViewModel>();

        var total = (ulong)Steps.Where(x => x.ItemType is not ItemType.EndOfSuite and not ItemType.None).Count() * 2;
        var progress = 0UL;
        var hasError = false;

        TaskBarProgressService.SetProgress(false, progress, total);

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
                        if (item.ItemType == ItemType.EndOfSuite)
                        {
                            installQueue.Enqueue(item);
                            item.Event.Set();
                            continue;
                        }

                        item.StepProgress = StepProgress.Loading;
                        await item.LoadInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);

                        if (cancellationToken.IsCancellationRequested)
                            break;

                        installQueue.Enqueue(item);
                        item.Event.Set();
                        item.StepProgress = StepProgress.Ready;

                        Interlocked.Increment(ref progress);
                        TaskBarProgressService.SetProgress(hasError, progress, total);
                    }
                    catch (Exception ex)
                    {
                        item.StepError = ex.Message;
                        item.StepProgress = StepProgress.Failed;
                        hasError = true;
                        TaskBarProgressService.SetProgress(hasError, progress, total);
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

                                if (item.IsVisible)
                                {
                                    Interlocked.Increment(ref progress);
                                    TaskBarProgressService.SetProgress(hasError, progress, total);
                                }
                            }
                            catch (Exception ex)
                            {
                                item.StepError = ex.Message;
                                item.StepProgress = StepProgress.Failed;
                                hasError = true;
                                TaskBarProgressService.SetProgress(hasError, progress, total);
                                _messenger.Send<CancelNotification>(new(true, ex));
                            }
                        }
                    }
                }
            }, cancellationToken);

            await Task.WhenAll(loadTask, installTask).ConfigureAwait(false);

            if (!hasError)
            {
                _messenger.Send<FinishNotification>();
            }
        }
        catch (Exception ex)
        {
            _messenger.Send<CancelNotification>(new(true, ex));
        }
    }

    public void Release()
    {
        if (LoadedCommand?.CanBeCanceled is true)
        {
            LoadedCommand.Cancel();
        }

        TaskBarProgressService.Reset();
    }
}
