using AsyncAwaitBestPractices;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;
using TableCloth3.Spork.Languages;
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
        AvaloniaWindowManager windowManager,
        AvaloniaViewModelManager viewModelManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;
        _viewModelManager = viewModelManager;

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
    private readonly AvaloniaViewModelManager _viewModelManager = default!;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<LoadingFailureNotification>.Receive(LoadingFailureNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var msgBox = MessageBoxManager.GetMessageBoxStandard(
                SporkStrings.UnexpectedErrorMessage_Title,
                string.Format(SporkStrings.UnexpectedErrorMessage_Arg0, message.OccurredException.Message),
                ButtonEnum.Ok, MsBox.Avalonia.Enums.Icon.Error);
            msgBox.ShowWindowDialogAsync(this).SafeFireAndForget();
        });
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
        installerWindow.ViewModel.TargetUrl = message.ViewModel.TargetUrl;

        foreach (var eachStep in message.ViewModel.Packages)
        {
            var eachVM = _viewModelManager.GetAvaloniaViewModel<InstallerStepItemViewModel>();
            eachVM.ServiceId = message.ViewModel.ServiceId;
            eachVM.PackageName = eachStep.PackageName;
            eachVM.PackageUrl = eachStep.PackageUrl;
            eachVM.PackageArguments = eachStep.PackageArguments;
            eachVM.RequireUserConfirmation = false;
            eachVM.RequireShellExecute = false;
            installerWindow.ViewModel.Steps.Add(eachVM);
        }

        var stSessVM = _viewModelManager.GetAvaloniaViewModel<InstallerStepItemViewModel>();
        stSessVM.ServiceId = message.ViewModel.ServiceId;
        stSessVM.PackageName = "StSessConfig";
        stSessVM.LocalFilePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
            "AhnLab", "Safe Transaction", "StSess.exe");
        stSessVM.PackageArguments = "/config";
        stSessVM.RequireUserConfirmation = true;
        stSessVM.RequireShellExecute = true;
        installerWindow.ViewModel.Steps.Add(stSessVM);

        installerWindow.ShowDialog(this);
    }
}
