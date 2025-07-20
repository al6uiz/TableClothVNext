using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using System.Diagnostics;
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
        WindowsSandboxComposer windowsSandboxComposer)
    {
        _messenger = messenger;
        _viewModelManager = viewModelManager;
        _appSettingsManager = appSettingsManager;
        _windowsSandboxComposer = windowsSandboxComposer;
    }

    public LauncherMainWindowViewModel() { }

    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaViewModelManager _viewModelManager = default!;
    private readonly AppSettingsManager _appSettingsManager = default!;
    private readonly WindowsSandboxComposer _windowsSandboxComposer = default!;

    public sealed record class AboutButtonMessage;

    public interface IAboutButtonMessageRecipient : IRecipient<AboutButtonMessage>;

    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    public sealed record class ManageFolderButtonMessage;

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
            var folderViewModel = _viewModelManager.GetAvaloniaViewModel<FolderManageWindowViewModel>();
            await _appSettingsManager.LoadAsync(folderViewModel, "folderListConfig.json", cancellationToken).ConfigureAwait(false);
            var wsbPath = await _windowsSandboxComposer.GenerateWindowsSandboxProfileAsync(
                this, folderViewModel, warnings, cancellationToken).ConfigureAwait(false);

            if (warnings.Any())
                _messenger.Send<NotifyWarningsMessage>(new NotifyWarningsMessage(warnings));

            using var process = Process.Start(new ProcessStartInfo(wsbPath)
            {
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal,
            });

            if (process == null)
            {
                _messenger.Send<NotifyErrorMessage>(new NotifyErrorMessage(
                    new Exception("Cannot start the Windows Sandbox process.")));
                return;
            }

            process.EnableRaisingEvents = true;
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
        => _messenger.Send<ManageFolderButtonMessage>();

    public override void PopulateForSerialization(IDictionary<string, object?> propertyBag)
    {
        propertyBag[nameof(UseMicrophone)] = UseMicrophone;
        propertyBag[nameof(UseWebCamera)] = UseWebCamera;
        propertyBag[nameof(SharePrinters)] = SharePrinters;
        propertyBag[nameof(MountNpkiFolders)] = MountNpkiFolders;
        propertyBag[nameof(MountSpecificFolders)] = MountSpecificFolders;
        base.PopulateForSerialization(propertyBag);
    }

    public override void PopulateForDeserialization(IReadOnlyDictionary<string, object?> propertyBag)
    {
        UseMicrophone = Convert.ToBoolean(propertyBag[nameof(UseMicrophone)]?.ToString());
        UseWebCamera = Convert.ToBoolean(propertyBag[nameof(UseWebCamera)]?.ToString());
        SharePrinters = Convert.ToBoolean(propertyBag[nameof(SharePrinters)]?.ToString());
        MountNpkiFolders = Convert.ToBoolean(propertyBag[nameof(MountNpkiFolders)]?.ToString());
        MountSpecificFolders = Convert.ToBoolean(propertyBag[nameof(MountSpecificFolders)]?.ToString());
        base.PopulateForDeserialization(propertyBag);
    }
}
