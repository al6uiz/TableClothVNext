using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Diagnostics;
using TableCloth3.Spork.ViewModels;
using static TableCloth3.Spork.ViewModels.InstallerProgressWindowViewModel;
using static TableCloth3.Spork.ViewModels.InstallerStepItemViewModel;

namespace TableCloth3;

public partial class InstallerProgressWindow :
    Window,
    ICancelNotificationRecipient,
    IFailureNotificationRecipient,
    IFinishNotificationRecipient,
    IUserConfirmationRecipient,
    IShowErrorRequestRecipient
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindow(
        InstallerProgressWindowViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        DataContext = _viewModel;

        _messenger.Register<CancelNotification>(this);
        _messenger.Register<FinishNotification>(this);
        _messenger.Register<UserConfirmationRequest>(this);
        _messenger.Register<ShowErrorRequest>(this);
        _messenger.Register<FailureNotification>(this);
    }

    public InstallerProgressWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly InstallerProgressWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    public InstallerProgressWindowViewModel ViewModel => _viewModel;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<CancelNotification>.Receive(CancelNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (message.DueToError)
            {
                var result = MessageBoxManager.GetMessageBoxStandard(
                    "Installation Cancelled",
                    $"Installation cancelled due to error. {message.FoundException}",
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Warning);
                result.ShowWindowDialogAsync(this);
            }

            Close();
        });
    }

    void IRecipient<FinishNotification>.Receive(FinishNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (message.HasError)
            {
                var result = MessageBoxManager.GetMessageBoxStandard(
                    "Installation Error",
                    "An error occurred during installation.",
                    ButtonEnum.Ok,
                    MsBox.Avalonia.Enums.Icon.Error);
                result.ShowWindowDialogAsync(this);
            }
            else
            {
                var targetUrl = ViewModel.TargetUrl;

                if (!string.IsNullOrWhiteSpace(targetUrl) &&
                    Uri.TryCreate(targetUrl, UriKind.Absolute, out var parsedTargetUrl) &&
                    parsedTargetUrl != null)
                {
                    Process.Start(new ProcessStartInfo(parsedTargetUrl.AbsoluteUri)
                    {
                        UseShellExecute = true,
                    });
                }

                Close();
            }
        });
    }

    void IRecipient<UserConfirmationRequest>.Receive(UserConfirmationRequest message)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var result = MessageBoxManager.GetMessageBoxStandard(
                "Confirmation",
                "Press OK to continue",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Info);

            await result.ShowWindowDialogAsync(this);
            message.ViewModel.ConfirmCommand.Execute(message.ViewModel);
        });
    }

    void IRecipient<ShowErrorRequest>.Receive(ShowErrorRequest message)
    {
        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            var result = MessageBoxManager.GetMessageBoxStandard(
                "Error",
                message.ViewModel.StepError,
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            await result.ShowWindowDialogAsync(this);
            message.ViewModel.ConfirmCommand.Execute(message.ViewModel);
        });
    }

    void IRecipient<FailureNotification>.Receive(FailureNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var result = MessageBoxManager.GetMessageBoxStandard(
                "Installation Failure",
                $"An error occurred during installation: {message.FoundException}",
                ButtonEnum.Ok,
                MsBox.Avalonia.Enums.Icon.Error);
            result.ShowWindowDialogAsync(this);
            Close();
        });
    }
}