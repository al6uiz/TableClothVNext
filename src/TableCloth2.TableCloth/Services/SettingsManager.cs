using TableCloth2.Models;
using TableCloth2.Shared.Services;
using TableCloth2.TableCloth.ViewModels;

namespace TableCloth2.TableCloth.Services;

public sealed class SettingsManager
{
    public SettingsManager(
        SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    private readonly SettingsService _settingsService;

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

        await _settingsService.SaveSettings(model, cancellationToken).ConfigureAwait(false);
    }

    public async Task ImportSettingsToViewModelAsync(
        SettingsViewModel viewModel,
        CancellationToken cancellationToken = default)
    {
        var model = await _settingsService.LoadSettings(cancellationToken).ConfigureAwait(false);

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
