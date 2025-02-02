using Microsoft.Extensions.Logging;
using System.Text.Json;
using TableCloth2.Models;
using TableCloth2.Services;
using TableCloth2.TableCloth.ViewModels;

namespace TableCloth2.TableCloth.Services;

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

    public async Task ExportSettingsFromViewModelAsync(
        SettingsViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var model = new SettingsModel
        {
            MountNPKICerts = viewModel.MountNPKICerts,
            EnableFolderMount = viewModel.EnableFolderMount,
            FolderMountList = viewModel.FolderMountList,

            EnableAudioInput = viewModel.EnableAudioInput,
            EnableVideoInput = viewModel.EnableVideoInput,
            EnablePrinterRedirection = viewModel.EnablePrinterRedirection,
            EnableVirtualizedGpu = viewModel.EnableVirtualizedGpu,
            UseCloudflareDns = viewModel.UseCloudflareDns,

            CollectSentryLog = viewModel.CollectSentryLog,
            CollectAnalytics = viewModel.CollectAnalytics,
        };

        await SaveSettings(model, cancellationToken).ConfigureAwait(false);
    }

    public async Task ImportSettingsToViewModelAsync(
        SettingsViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var model = await LoadSettings(cancellationToken).ConfigureAwait(false);

        viewModel.MountNPKICerts = model.MountNPKICerts;
        viewModel.EnableFolderMount = model.EnableFolderMount;
        viewModel.FolderMountList = model.FolderMountList;

        viewModel.EnableAudioInput = model.EnableAudioInput;
        viewModel.EnableVideoInput = model.EnableVideoInput;
        viewModel.EnablePrinterRedirection = model.EnablePrinterRedirection;
        viewModel.EnableVirtualizedGpu = model.EnableVirtualizedGpu;
        viewModel.UseCloudflareDns = model.UseCloudflareDns;

        viewModel.CollectSentryLog = model.CollectSentryLog;
        viewModel.CollectAnalytics = model.CollectAnalytics;
    }
}
