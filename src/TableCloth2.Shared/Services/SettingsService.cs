using Microsoft.Extensions.Logging;
using System.Text.Json;
using TableCloth2.Models;
using TableCloth2.Services;

namespace TableCloth2.Shared.Services;

public sealed class SettingsService
{
    public SettingsService(
        KnownPathsService knownPathsService,
        ILogger<SettingsService> logger
        ) : base()
    {
        _knownPathsService = knownPathsService;
        _logger = logger;
    }

    private readonly KnownPathsService _knownPathsService;
    private readonly ILogger _logger;

    public async Task<SettingsModel> LoadSettings(
        CancellationToken cancellationToken = default)
    {
        var model = default(SettingsModel);
        var filePath = _knownPathsService.EnsureTableClothSettingsDirectoryExists().Combine("settings.js");

        try
        {
            using var settingsFile = File.OpenRead(filePath);
            model = await JsonSerializer.DeserializeAsync<SettingsModel>(
                settingsFile, cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Cannot load settings file from '{filePath}'.",
                filePath);
        }

        if (model == null)
        {
            model = new SettingsModel();
            _logger.LogWarning("Create default configuration instead.");
        }

        return model;
    }

    public async Task SaveSettings(
        SettingsModel model,
        CancellationToken cancellationToken = default)
    {
        var filePath = _knownPathsService.EnsureTableClothSettingsDirectoryExists().Combine("settings.js");

        using var settingsFile = File.Open(filePath, FileMode.Create);
        await JsonSerializer.SerializeAsync(
            settingsFile, model,
            cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
