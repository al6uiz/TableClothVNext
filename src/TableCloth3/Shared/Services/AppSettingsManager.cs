using Microsoft.Extensions.Logging;
using System.Text.Json;
using TableCloth3.Shared.Converters;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Shared.Services;

public sealed class AppSettingsManager
{
    public AppSettingsManager(
        ILogger<AppSettingsManager> logger,
        LocationService locationService,
        ObjectToInferredTypeConverter objectToInferredTypeConverter)
        : base()
    {
        _logger = logger;
        _locationService = locationService;
        _objectToInferredTypeConverter = objectToInferredTypeConverter;
    }

    private readonly ILogger<AppSettingsManager> _logger = default!;
    private readonly LocationService _locationService = default!;
    private readonly ObjectToInferredTypeConverter _objectToInferredTypeConverter = default!;

    public async Task LoadAsync<TBaseViewModel>(
        TBaseViewModel viewModel,
        string fileName,
        CancellationToken cancellationToken = default)
        where TBaseViewModel : BaseViewModel
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = viewModel.GetType().Name;
        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            fileName = string.Concat(fileName, ".json");

        var directoryPath = _locationService.EnsureAppDataDirectoryCreated().FullName;
        var filePath = Path.Combine(directoryPath, fileName);

        try
        {
            using var fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var options = new JsonSerializerOptions
            {
                Converters = { _objectToInferredTypeConverter, },
            };
            var items = await JsonSerializer.DeserializeAsync<Dictionary<string, object?>>(
                fileStream, options, cancellationToken)
                .ConfigureAwait(false);
            viewModel.PopulateForDeserialization(items ?? new Dictionary<string, object?>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot load settings from file '{filepath}' due to error.", filePath);
        }
    }

    public async Task SaveAsync<TBaseViewModel>(
        TBaseViewModel viewModel,
        string fileName,
        CancellationToken cancellationToken = default)
        where TBaseViewModel : BaseViewModel
    {
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = viewModel.GetType().Name;
        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            fileName = string.Concat(fileName, ".json");

        var directoryPath = _locationService.EnsureAppDataDirectoryCreated().FullName;
        var filePath = Path.Combine(directoryPath, fileName);
        using var fileStream = File.Open(filePath, FileMode.Create, FileAccess.ReadWrite);

        var options = new JsonSerializerOptions
        {
            Converters = { _objectToInferredTypeConverter, },
        };

        var items = default(Dictionary<string, object?>);

        try
        {
            items = await JsonSerializer.DeserializeAsync<Dictionary<string, object?>>(
                fileStream, options, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Cannot load settings from file '{filepath}' due to error.", filePath);
        }

        items ??= new Dictionary<string, object?>();
        viewModel.PopulateForSerialization(items);

        await JsonSerializer.SerializeAsync(fileStream, items, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
}
