using AsyncAwaitBestPractices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Windows.Input;
using TableCloth2.Contracts;
using TableCloth2.Shared.Services;
using TableCloth2.TableCloth.Contracts;
using TableCloth2.TableCloth.Services;
using WindowsFormsLifetime;

namespace TableCloth2.TableCloth.ViewModels;

public sealed class TableClothViewModel : ViewModelBase
{
    public TableClothViewModel(
        ILogger<TableClothViewModel> logger,
        Configurations configurations,
        IHostApplicationLifetime lifetime,
        IFormProvider formProvider,
        SettingsService settingsService,
        SettingsManager settingsManager,
        ISandboxComposer sandboxComposer,
        IMessageBoxService messageBoxService,
        SessionService sessionService)
    {
        _logger = logger;
        _configurations = configurations;
        _lifetime = lifetime;
        _formProvider = formProvider;
        _settingsService = settingsService;
        _settingsManager = settingsManager;
        _sandboxComposer = sandboxComposer;
        _messageBoxService = messageBoxService;
        _sessionService = sessionService;

        _initializeEvent = new RelayCommand(Initialize);
        _cleanupEvent = new RelayCommand(Cleanup);

        _changeSettingsCommand = new RelayCommand(ChangeSettings);
        _launchCommand = new RelayCommand(Launch);
    }

    private readonly ILogger _logger;
    private readonly Configurations _configurations;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IFormProvider _formProvider;
    private readonly SettingsService _settingsService;
    private readonly SettingsManager _settingsManager;
    private readonly ISandboxComposer _sandboxComposer;
    private readonly IMessageBoxService _messageBoxService;
    private readonly SessionService _sessionService;

    private readonly RelayCommand _initializeEvent;
    private readonly RelayCommand _cleanupEvent;

    private readonly RelayCommand _changeSettingsCommand;
    private readonly RelayCommand _launchCommand;

    internal ICommand InitializeEvent => _initializeEvent;
    internal ICommand CleanupEvent => _cleanupEvent;

    public ICommand ChangeSettingsCommand => _changeSettingsCommand;
    public ICommand LaunchCommand => _launchCommand;

    private void Initialize(object? _)
        => InitializeAsync(_).SafeFireAndForget();

    private async Task InitializeAsync(object? _)
    {
        var bootstrapForm = await _formProvider.GetFormAsync<BootstrapForm>();
        if (bootstrapForm.ShowDialog() != DialogResult.OK)
        {
            MessageBox.Show("Bootstrap failed.");
            _lifetime.StopApplication();
            return;
        }
    }

    private void Cleanup(object? _)
    {
        var targetDirectory = _sessionService.CreateSessionDirectory();

        try { targetDirectory.Delete(true); }
        catch (Exception ex) { _logger.LogWarning(ex, "Cannot delete '{targetDirectory}'.", targetDirectory.FullName); }
    }

    private void ChangeSettings(object? _)
        => ChangeSettingsAsync(_).SafeFireAndForget();

    private async Task ChangeSettingsAsync(object? _)
    {
        using var settingsForm = await _formProvider.GetFormAsync<SettingsForm>();

        await _settingsManager.ImportSettingsToViewModelAsync(settingsForm.ViewModel);

        if (settingsForm.ShowDialog() != DialogResult.OK)
            return;

        await _settingsManager.ExportSettingsFromViewModelAsync(settingsForm.ViewModel);
    }

    private void Launch(object? _)
        => LaunchSettingsAsync(_).SafeFireAndForget();

    private async Task LaunchSettingsAsync(object? _)
    {
        var candidates = new string[]
        {
            "WindowsSandbox",
            "WindowsSandboxServer",
            "WindowsSandboxRemoteSession",
        };

        var sandboxRunnning = Process.GetProcesses()
            .Any(x => candidates.Contains(x.ProcessName, StringComparer.OrdinalIgnoreCase));

        if (sandboxRunnning)
        {
            await _messageBoxService.ShowWarningAsync("Sandbox is running.", "TableCloth2");
            return;
        }

        var settings = await _settingsService.LoadSettings();
        var sandboxFile = _sandboxComposer.CreateSandboxSpec(settings);
        var systemDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Windows);

        var cmdInterpreter = Path.Combine(systemDirectory, "System32", "cmd.exe");
        if (!File.Exists(cmdInterpreter))
            throw new Exception($"'{cmdInterpreter}' is not exists.");

        var winSandbox = Path.Combine(systemDirectory, "System32", "WindowsSandbox.exe");
        if (!File.Exists(winSandbox))
            throw new Exception($"'{winSandbox}' is not exists.");

        using var process = new Process()
        {
            StartInfo = new ProcessStartInfo(cmdInterpreter, $"/c start /wait {winSandbox} {sandboxFile.FullName}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
            },
            EnableRaisingEvents = true,
        };
        process.Start();

        await process.WaitForExitAsync();
    }
}
