using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
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
        try
        {
            foreach (var eachStep in Steps)
                await eachStep.LoadInstallStepCommand.ExecuteAsync(cancellationToken);

            foreach (var eachStep in Steps)
                await eachStep.PerformInstallStepCommand.ExecuteAsync(cancellationToken);

            _messenger.Send<FinishNotification>();
        }
        catch (Exception ex)
        {
            _messenger.Send<CancelNotification>(new(true, ex));
        }
    }
}

public sealed partial class InstallerStepItemViewModel : BaseViewModel
{
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

    partial void OnStepErrorChanged(string value)
        => OnPropertyChanged(nameof(HasError));

    partial void OnStepProgressChanged(StepProgress value)
        => OnPropertyChanged(nameof(StatusText));

    [RelayCommand]
    private async Task LoadInstallStep(CancellationToken cancellationToken = default)
    {
        try
        {
            StepProgress = StepProgress.None;
            
            await Task.Delay(TimeSpan.FromSeconds(2d), cancellationToken).ConfigureAwait(false);

            StepProgress = StepProgress.Ready;
        }
        catch (Exception ex)
        {
            StepError = ex.Message;
            StepProgress = StepProgress.Failed;
        }
    }

    [RelayCommand]
    private async Task PerformInstallStep(CancellationToken cancellationToken = default)
    {
        try
        {
            StepProgress = StepProgress.Installing;

            await Task.Delay(TimeSpan.FromSeconds(1d), cancellationToken).ConfigureAwait(false);

            StepProgress = StepProgress.Succeed;
        }
        catch (Exception ex)
        {
            StepError = ex.Message;
            StepProgress = StepProgress.Failed;
        }
    }

    public string StatusText => StepProgress switch
    {
        StepProgress.Loading => "⏳",
        StepProgress.Ready => "📦",
        StepProgress.Installing => "🛠️",
        StepProgress.Succeed => "✔️",
        StepProgress.Failed => "❌",
        StepProgress.Unknown => "❔",
        _ => "⬜",
    };

    public bool HasError => !string.IsNullOrWhiteSpace(StepError);
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
