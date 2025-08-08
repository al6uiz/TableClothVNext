using TableCloth3.Launcher.Models;
using TableCloth3.Shared.Services;

namespace TableCloth3.Launcher.Services;

public sealed class LauncherSettingsManager
{
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
            "launcherConfig.json",
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
            "launcherConfig.json",
            cancellationToken).ConfigureAwait(false);
    }
}
