using Microsoft.Extensions.Logging;
using System.Text.Json;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Shared.Services;

public sealed class AppSettingsManager
{
    public AppSettingsManager(
        ILogger<AppSettingsManager> logger,
        LocationService locationService)
        : base()
    {
        _logger = logger;
        _locationService = locationService;
    }

    private readonly ILogger<AppSettingsManager> _logger;
    private readonly LocationService _locationService = default!;

    public async Task LoadAsync<TBaseViewModel>(
        TBaseViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TBaseViewModel : BaseViewModel
    {
        var directoryPath = _locationService.EnsureAppDataDirectoryCreated().FullName;
        var filePath = Path.Combine(directoryPath, "config.json");

        try
        {
            using var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var items = await JsonSerializer.DeserializeAsync<Dictionary<string, object?>>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
            viewModel.PopulateForDeserialization(items ?? new Dictionary<string, object?>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot load settings from file '{filepath}' due to error.", filePath);
        }
    }

    public async Task SaveAsync<TBaseViewModel>(
        TBaseViewModel viewModel,
        CancellationToken cancellationToken = default)
        where TBaseViewModel : BaseViewModel
    {
        var directoryPath = _locationService.EnsureAppDataDirectoryCreated().FullName;
        var filePath = Path.Combine(directoryPath, "config.json");
        using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.Write);

        var items = new Dictionary<string, object?>();
        viewModel.PopulateForSerialization(items);

        await JsonSerializer.SerializeAsync(fileStream, items, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
