using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Launcher.ViewModels;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;

using static TableCloth3.Launcher.ViewModels.LauncherMainWindowViewModel;

namespace TableCloth3.Launcher.Windows;

public partial class LauncherMainWindow :
    Window,
    IAboutButtonMessageRecipient,
    ICloseButtonMessageRecipient,
    IManageFolderButtonMessageRecipient
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

        _messenger.Register<AboutButtonMessage>(this);
        _messenger.Register<CloseButtonMessage>(this);
        _messenger.Register<ManageFolderButtonMessage>(this);
    }

    public LauncherMainWindow()
        : base()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        _appSettingsManager.LoadAsync(_viewModel).SafeFireAndForget();
        base.OnLoaded(e);
    }

    protected override void OnClosed(EventArgs e)
    {
        _messenger?.UnregisterAll(this);
        _appSettingsManager.SaveAsync(_viewModel).SafeFireAndForget();
        base.OnClosed(e);
    }

    private readonly LauncherMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;
    private readonly AppSettingsManager _appSettingsManager = default!;

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
}
