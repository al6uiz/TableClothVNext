using AsyncAwaitBestPractices;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Diagnostics;
using TableCloth3.Launcher.Models;
using TableCloth3.Launcher.Services;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Launcher.ViewModels;

public sealed partial class LauncherMainWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public LauncherMainWindowViewModel(
        IMessenger messenger,
        AvaloniaViewModelManager viewModelManager,
        AppSettingsManager appSettingsManager,
        WindowsSandboxComposer windowsSandboxComposer,
        TableClothCatalogService tableClothCatalogService,
        ProcessManagerFactory processManagerFactory)
    {
        _messenger = messenger;
        _viewModelManager = viewModelManager;
        _appSettingsManager = appSettingsManager;
        _windowsSandboxComposer = windowsSandboxComposer;
        _tableClothCatalogService = tableClothCatalogService;
        _processManagerFactory = processManagerFactory;
    }

    public LauncherMainWindowViewModel() { }

    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaViewModelManager _viewModelManager = default!;
    private readonly AppSettingsManager _appSettingsManager = default!;
    private readonly WindowsSandboxComposer _windowsSandboxComposer = default!;
    private readonly TableClothCatalogService _tableClothCatalogService = default!;
    private readonly ProcessManagerFactory _processManagerFactory = default!;

    public sealed record class AboutButtonMessage;

    public interface IAboutButtonMessageRecipient : IRecipient<AboutButtonMessage>;

    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    public sealed record class ManageFolderButtonMessage(ObservableCollection<string> Folders);

    public interface IManageFolderButtonMessageRecipient : IRecipient<ManageFolderButtonMessage>;

    public sealed record class NotifyErrorMessage(Exception FoundException);

    public interface INotifyErrorMessageRecipient : IRecipient<NotifyErrorMessage>;

    public sealed record class NotifyWarningsMessage(IEnumerable<string> FoundWarnings);

    public interface INotifyWarningsMessageRecipient : IRecipient<NotifyWarningsMessage>;

    [ObservableProperty]
    private bool _useMicrophone = false;

    [ObservableProperty]
    private bool _useWebCamera = false;

    [ObservableProperty]
    private bool _sharePrinters = false;

    [ObservableProperty]
    private bool _mountNpkiFolders = true;

    [ObservableProperty]
    private bool _mountSpecificFolders = false;

    [ObservableProperty]
    private ObservableCollection<string> _folders = new ObservableCollection<string>();

    [ObservableProperty]
    private bool _loading = false;

    [RelayCommand]
    private void AboutButton()
        => _messenger.Send<AboutButtonMessage>();

    [RelayCommand(AllowConcurrentExecutions = false)]
    private async Task LaunchButton(CancellationToken cancellationToken = default)
    {
        try
        {
            var processList = Process.GetProcesses().Select(x => x.ProcessName);
            var lookupList = new List<string> { "WindowsSandbox", "WindowsSandboxRemoteSession", "WindowsSandboxServer", };
            foreach (var eachProcessName in processList)
                if (lookupList.Contains(eachProcessName, StringComparer.OrdinalIgnoreCase))
                    throw new Exception("Only one Windows Sandbox session allowed.");

            var warnings = new List<string>();
            var config = await _appSettingsManager.LoadAsync<LauncherSerializerContext, LauncherSettingsModel>(LauncherSerializerContext.Default, "launcherConfig.json", cancellationToken).ConfigureAwait(false);
            var wsbPath = await _windowsSandboxComposer.GenerateWindowsSandboxProfileAsync(
                this, warnings, cancellationToken).ConfigureAwait(false);

            if (warnings.Any())
                _messenger.Send<NotifyWarningsMessage>(new NotifyWarningsMessage(warnings));

            var windowsSandboxExecPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.System),
                "WindowsSandbox.exe");

            using var process = _processManagerFactory.CreateCmdShellProcess(windowsSandboxExecPath, wsbPath);

            if (process.Start())
                await process.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _messenger.Send<NotifyErrorMessage>(new NotifyErrorMessage(ex));
        }
    }

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonMessage>();

    [RelayCommand]
    private void ManageFolderButton()
        => _messenger.Send<ManageFolderButtonMessage>(new ManageFolderButtonMessage(Folders));

    [RelayCommand]
    private void Loaded(CancellationToken cancellationToken = default)
    {
        if (Design.IsDesignMode)
            return;

        Loading = true;

        Task.WhenAll([
            _tableClothCatalogService.DownloadCatalogAsync(cancellationToken),
            _tableClothCatalogService.DownloadImagesAsync(cancellationToken),
        ])
        .ContinueWith(x =>
        {
            Loading = false;
        })
        .SafeFireAndForget(ex =>
        {
            // TODO: Notify Error Event
        });
    }
}
