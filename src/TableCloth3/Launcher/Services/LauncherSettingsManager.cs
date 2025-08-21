using TableCloth3.Launcher.Models;
using TableCloth3.Shared.Services;

namespace TableCloth3.Launcher.Services;

public sealed class LauncherSettingsManager
{
    private const string SETTINGS_FILENAME = "launcherConfig.json";

    public LauncherSettingsManager(AppSettingsManager appSettingsManager)
    {
        _appSettingsManager = appSettingsManager;
    }

    private readonly AppSettingsManager _appSettingsManager = default!;

    public async Task<LauncherSettingsModel?> LoadSettingsAsync(
        CancellationToken cancellationToken = default)
    {
        return await _appSettingsManager.LoadAsync<LauncherSerializerContext, LauncherSettingsModel>(
            LauncherSerializerContext.Default,
            SETTINGS_FILENAME,
            cancellationToken).ConfigureAwait(false);
    }

    public async Task SaveSettingsAsync(
        LauncherSettingsModel settings,
        CancellationToken cancellationToken = default)
    {
        if (settings == null)
            throw new ArgumentNullException(nameof(settings));
        await _appSettingsManager.SaveAsync<LauncherSerializerContext, LauncherSettingsModel>(
            LauncherSerializerContext.Default,
            settings,
            SETTINGS_FILENAME,
            cancellationToken).ConfigureAwait(false);
    }
}
