using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Rendering;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using TableCloth3.Launcher.Languages;
using TableCloth3.Launcher.Models;
using TableCloth3.Launcher.Services;
using TableCloth3.Launcher.ViewModels;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;

using static TableCloth3.Launcher.ViewModels.LauncherMainWindowViewModel;

namespace TableCloth3.Launcher.Windows;

public partial class LauncherMainWindow :
    Window,
    IShowDisclaimerWindowMessageRecipient,
    IAboutButtonMessageRecipient,
    ICloseButtonMessageRecipient,
    IManageFolderButtonMessageRecipient,
    INotifyErrorMessageRecipient,
    INotifyWarningsMessageRecipient
{
    [ActivatorUtilitiesConstructor]
    public LauncherMainWindow(
        LauncherMainWindowViewModel viewModel,
        IMessenger messenger,
        AvaloniaWindowManager windowManager,
        LauncherSettingsManager launcherSettingsManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;
        _launcherSettingsManager = launcherSettingsManager;

        DataContext = _viewModel;

        _messenger.Register<ShowDisclaimerWindowMessage>(this);
        _messenger.Register<AboutButtonMessage>(this);
        _messenger.Register<CloseButtonMessage>(this);
        _messenger.Register<ManageFolderButtonMessage>(this);
        _messenger.Register<NotifyErrorMessage>(this);
        _messenger.Register<NotifyWarningsMessage>(this);
    }

    public LauncherMainWindow()
        : base()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _launcherSettingsManager.LoadSettingsAsync()
            .ContinueWith(x =>
            {
                var config = x.Result ?? new LauncherSettingsModel();
                _viewModel.UseMicrophone = config.UseMicrophone;
                _viewModel.UseWebCamera = config.UseWebCamera;
                _viewModel.SharePrinters = config.SharePrinters;
                _viewModel.MountNpkiFolders = config.MountNpkiFolders;
                _viewModel.MountSpecificFolders = config.MountSpecificFolders;
                _viewModel.DisclaimerAccepted = config.DisclaimerAccepted;

                _viewModel.Folders.Clear();
                foreach (var eachDir in config.Folders)
                    _viewModel.Folders.Add(eachDir);

                var requireAcknowledge = false;
                if (_viewModel.DisclaimerAccepted.HasValue)
                {
                    if ((DateTime.UtcNow - _viewModel.DisclaimerAccepted.Value).TotalDays > 7)
                    {
                        requireAcknowledge = true;
                        _viewModel.DisclaimerAccepted = null;
                    }
                }
                else
                    requireAcknowledge = true;

                if (requireAcknowledge)
                {
                    Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        var window = _windowManager.GetAvaloniaWindow<DisclaimerWindow>();
                        await window.ShowDialog(this);
                        _viewModel.DisclaimerAccepted = DateTime.UtcNow;
                    });
                }
            })
            .SafeFireAndForget();
        base.OnLoaded(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _messenger?.UnregisterAll(this);

        var config = new LauncherSettingsModel
        {
            UseMicrophone = _viewModel.UseMicrophone,
            UseWebCamera = _viewModel.UseWebCamera,
            SharePrinters = _viewModel.SharePrinters,
            MountNpkiFolders = _viewModel.MountNpkiFolders,
            MountSpecificFolders = _viewModel.MountSpecificFolders,
            Folders = _viewModel.Folders.ToArray(),
            DisclaimerAccepted = _viewModel.DisclaimerAccepted,
        };

        _launcherSettingsManager.SaveSettingsAsync(config).SafeFireAndForget();

        base.OnClosed(e);
    }

    private readonly LauncherMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;
    private readonly LauncherSettingsManager _launcherSettingsManager = default!;

    void IRecipient<ShowDisclaimerWindowMessage>.Receive(ShowDisclaimerWindowMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var window = _windowManager.GetAvaloniaWindow<DisclaimerWindow>();
            window.ShowDialog(this);
        });
    }

    void IRecipient<AboutButtonMessage>.Receive(AboutButtonMessage message)
    {
        var aboutWindow = _windowManager.GetAvaloniaWindow<AboutWindow>();
        aboutWindow.ShowDialog(this);
    }

    void IRecipient<CloseButtonMessage>.Receive(CloseButtonMessage message)
    {
        Close();
    }

    void IRecipient<ManageFolderButtonMessage>.Receive(ManageFolderButtonMessage message)
    {
        var folderManageWindow = _windowManager.GetAvaloniaWindow<FolderManageWindow>();

        folderManageWindow.ViewModel.Folders.Clear();
        foreach (var eachDir in message.Folders)
            folderManageWindow.ViewModel.Folders.Add(eachDir);

        folderManageWindow.ShowDialog(this).ContinueWith(x =>
        {
            if (x.IsCompletedSuccessfully)
            {
                _viewModel.Folders.Clear();

                foreach (var eachDir in folderManageWindow.ViewModel.Folders)
                    _viewModel.Folders.Add(eachDir);
            }
        }).SafeFireAndForget();
    }

    void IRecipient<NotifyErrorMessage>.Receive(NotifyErrorMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var messageBoxParam = new MessageBoxStandardParams()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentTitle = LauncherStrings.ErrorTitle,
                ContentMessage = message.FoundException.Message,
                Icon = MsBox.Avalonia.Enums.Icon.Error,
                ButtonDefinitions = MsBox.Avalonia.Enums.ButtonEnum.Ok,
                EnterDefaultButton = MsBox.Avalonia.Enums.ClickEnum.Ok,
                EscDefaultButton = MsBox.Avalonia.Enums.ClickEnum.Cancel,
            };

            MessageBoxManager
                .GetMessageBoxStandard(messageBoxParam)
                .ShowWindowDialogAsync(this)
                .SafeFireAndForget();
        });
    }

    void IRecipient<NotifyWarningsMessage>.Receive(NotifyWarningsMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var messageBoxParam = new MessageBoxStandardParams()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentTitle = LauncherStrings.WarningTitle,
                ContentMessage = string.Join(Environment.NewLine, message.FoundWarnings),
                Icon = MsBox.Avalonia.Enums.Icon.Warning,
                ButtonDefinitions = MsBox.Avalonia.Enums.ButtonEnum.Ok,
                EnterDefaultButton = MsBox.Avalonia.Enums.ClickEnum.Ok,
                EscDefaultButton = MsBox.Avalonia.Enums.ClickEnum.Cancel,
            };

            MessageBoxManager
                .GetMessageBoxStandard(messageBoxParam)
                .ShowWindowDialogAsync(this)
                .SafeFireAndForget();
        });
    }
}
