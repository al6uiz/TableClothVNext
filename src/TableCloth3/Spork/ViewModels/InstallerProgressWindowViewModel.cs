using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
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
            var progress = (StepProgress)(i % Enum.GetValues<StepProgress>().Count());
            Steps.Add(new()
            {
                StepProgress = progress,
                PackageName = $"Item {i+1}",
                PackageUrl = "https://yourtablecloth.app/",
                PackageArguments = "/S",
                StepError = progress == StepProgress.Failed ? "An error occurred while processing this step." : string.Empty,
                Percentage = Random.Shared.Next(0, 100),
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];

    [ObservableProperty]
    private string _targetUrl = "https://yourtablecloth.app/";

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
        try
        {
            foreach (var item in Steps)
            {
                try
                {
                    item.StepProgress = StepProgress.Loading;
                    await item.LoadInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                    item.StepProgress = StepProgress.Ready;
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

            if (!string.IsNullOrWhiteSpace(TargetUrl) &&
                Uri.TryCreate(TargetUrl, UriKind.Absolute, out var parsedTargetUrl) &&
                parsedTargetUrl != null)
            {
                Process.Start(new ProcessStartInfo(parsedTargetUrl.AbsoluteUri)
                {
                    UseShellExecute = true,
                });
            }

            _messenger.Send<FinishNotification>();
        }
        catch (Exception ex)
        {
            _messenger.Send<CancelNotification>(new(true, ex));
        }
    }
}
