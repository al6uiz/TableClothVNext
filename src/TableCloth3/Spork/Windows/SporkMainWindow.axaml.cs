using AsyncAwaitBestPractices;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;
using TableCloth3.Spork.ViewModels;
using static TableCloth3.Spork.ViewModels.SporkMainWindowViewModel;
using static TableCloth3.Spork.ViewModels.TableClothCatalogItemViewModel;

namespace TableCloth3.Spork.Windows;

public partial class SporkMainWindow :
    Window,
    IRecipient<LoadingFailureNotification>,
    IRecipient<AboutButtonRequest>,
    IRecipient<CloseButtonRequest>,
    IRecipient<LaunchSiteRequest>
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindow(
        SporkMainWindowViewModel viewModel,
        IMessenger messenger,
        AvaloniaWindowManager windowManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;

        _messenger.Register<LoadingFailureNotification>(this);
        _messenger.Register<AboutButtonRequest>(this);
        _messenger.Register<CloseButtonRequest>(this);
        _messenger.Register<LaunchSiteRequest>(this);

        DataContext = _viewModel;
    }

    public SporkMainWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly SporkMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<LoadingFailureNotification>.Receive(LoadingFailureNotification message)
    {
        var msgBox = MessageBoxManager.GetMessageBoxStandard(
            "Unexpected error occurred",
            $"Unexpected error occurred: {message.OccurredException.Message}",
            ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
        msgBox.ShowWindowDialogAsync(this).SafeFireAndForget();
    }

    void IRecipient<AboutButtonRequest>.Receive(AboutButtonRequest message)
    {
        var aboutWindow = _windowManager.GetAvaloniaWindow<AboutWindow>();
        aboutWindow.ShowDialog(this);
    }

    void IRecipient<CloseButtonRequest>.Receive(CloseButtonRequest message)
    {
        Close();
    }

    void IRecipient<LaunchSiteRequest>.Receive(LaunchSiteRequest message)
    {
        var installerWindow = _windowManager.GetAvaloniaWindow<InstallerProgressWindow>();

        foreach (var eachStep in message.ViewModel.Packages)
        {
            installerWindow.ViewModel.Steps.Add(new InstallerStepItemViewModel()
            {
                PackageName = eachStep.PackageName,
                PackageUrl = eachStep.PackageUrl,
                PackageArguments = eachStep.PackageArguments,
            });
        }

        installerWindow.ShowDialog(this);
    }
}
