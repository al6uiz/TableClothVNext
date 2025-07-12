using Avalonia.Controls;
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
        AvaloniaWindowManager windowManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;

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

    protected override void OnClosed(EventArgs e)
    {
        _messenger?.UnregisterAll(this);
        base.OnClosed(e);
    }

    private readonly LauncherMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;

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
