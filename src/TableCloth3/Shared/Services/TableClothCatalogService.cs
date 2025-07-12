using System.Xml.Linq;
using TableCloth3.Shared.Languages;

namespace TableCloth3.Shared.Services;

public sealed class TableClothCatalogService
{
    public TableClothCatalogService(
        IHttpClientFactory httpClientFactory,
        ArchiveExpander archiveExpander,
        LocationService locationService)
        : base()
    {
        _httpClientFactory = httpClientFactory;
        _archiveExpander = archiveExpander;
        _locationService = locationService;
    }

    private readonly IHttpClientFactory _httpClientFactory = default!;
    private readonly ArchiveExpander _archiveExpander = default!;
    private readonly LocationService _locationService = default!;

    public async Task<XDocument> LoadCatalogAsync(
        CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateCatalogHttpClient();
        using var contentStream = await httpClient.GetStreamAsync($"/TableClothCatalog/Catalog.xml?ts={Uri.EscapeDataString(DateTime.UtcNow.Ticks.ToString())}", cancellationToken).ConfigureAwait(false);
        return await XDocument.LoadAsync(contentStream, default, cancellationToken).ConfigureAwait(false);
    }

    public string GetCatalogDisplayName(string? rawName)
        => rawName?.ToUpperInvariant() switch
        {
            "BANKING" => SharedStrings.BankingCategoryDisplayName,
            "FINANCING" => SharedStrings.FinancingCategoryDisplayName,
            "SECURITY" => SharedStrings.SecurityCategoryDisplayName,
            "CREDITCARD" => SharedStrings.CreditCardCategoryDisplayName,
            "INSURANCE" => SharedStrings.InsuranceCategoryDisplayName,
            "GOVERNMENT" => SharedStrings.GovernmentCategoryDisplayName,
            "EDUCATION" => SharedStrings.EducationCategoryDisplayName,
            _ => SharedStrings.OtherCategoryDisplayName,
        };

    public async Task LoadImagesAsync(
        CancellationToken cancellationToken = default)
    {
        // TODO: ZIP 파일 해시 값 비교 동작 추가
        var httpClient = _httpClientFactory.CreateCatalogHttpClient();
        using var contentStream = await httpClient.GetStreamAsync($"/TableClothCatalog/Images.zip?ts={Uri.EscapeDataString(DateTime.UtcNow.Ticks.ToString())}", cancellationToken).ConfigureAwait(false);

        var targetDirectoryToExtract = _locationService.EnsureImagesDirectoryCreated().FullName;
        var downloadPath = Path.Combine(targetDirectoryToExtract, "Images.zip");
        using var localStream = File.Open(downloadPath, FileMode.Create, FileAccess.ReadWrite);
        await contentStream.CopyToAsync(localStream, cancellationToken).ConfigureAwait(false);
        localStream.Seek(0L, SeekOrigin.Begin);

        targetDirectoryToExtract = Directory.CreateDirectory(targetDirectoryToExtract).FullName;
        await _archiveExpander.ExpandArchiveAsync(localStream, targetDirectoryToExtract, cancellationToken).ConfigureAwait(false);
    }

    public string GetLocalImagePath(string serviceId)
        => Path.GetFullPath(Path.Combine(_locationService.EnsureImagesDirectoryCreated().FullName, serviceId + ".png"));

    public string GetLocalIconPath(string serviceId)
        => Path.GetFullPath(Path.Combine(_locationService.EnsureImagesDirectoryCreated().FullName, serviceId + ".ico"));
}
