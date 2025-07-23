using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using TableCloth3.Launcher.Models;
using TableCloth3.Launcher.ViewModels;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;

using static TableCloth3.Launcher.ViewModels.LauncherMainWindowViewModel;

namespace TableCloth3.Launcher.Windows;

public partial class LauncherMainWindow :
    AppWindow,
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
        AppSettingsManager appSettingsManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;
        _appSettingsManager = appSettingsManager;

        DataContext = _viewModel;

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        ShowAsDialog = true;
    
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
        _appSettingsManager?.LoadAsync<LauncherSerializerContext, LauncherSettingsModel>(LauncherSerializerContext.Default, "launcherConfig.json")
            .ContinueWith(x =>
            {
                _config = x.Result ?? new LauncherSettingsModel();
                _viewModel.ImportFromModel(_config);
            })
            .SafeFireAndForget();
        base.OnLoaded(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _messenger?.UnregisterAll(this);

        _viewModel.ExportToModel(_config);
        _appSettingsManager.SaveAsync(
            LauncherSerializerContext.Default, _config,
            "launcherConfig.json").SafeFireAndForget();

        base.OnClosed(e);
    }

    private readonly LauncherMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;
    private readonly AppSettingsManager _appSettingsManager = default!;

    private LauncherSettingsModel _config = default!;

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
        folderManageWindow.ShowDialog(this);
    }

    void IRecipient<NotifyErrorMessage>.Receive(NotifyErrorMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var messageBoxParam = new MessageBoxStandardParams()
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentTitle = "Error",
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
                ContentTitle = "Warning",
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
