using Avalonia.Xaml.Interactions.Custom;
using System.IO.Compression;
using System.Xml.Linq;
using TableCloth3.Shared;
using TableCloth3.Shared.Languages;
using TableCloth3.Shared.Services;
using TableCloth3.Spork.Languages;

namespace TableCloth3.Spork.Services;

public sealed class TableClothCatalogService
{
    public TableClothCatalogService(
        IHttpClientFactory httpClientFactory,
        ArchiveExpander archiveExpander)
        : base()
    {
        _httpClientFactory = httpClientFactory;
        _archiveExpander = archiveExpander;
    }

    private readonly IHttpClientFactory _httpClientFactory = default!;
    private readonly ArchiveExpander _archiveExpander = default!;

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
        string targetDirectoryToExtract,
        CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateCatalogHttpClient();
        using var contentStream = await httpClient.GetStreamAsync($"/TableClothCatalog/Images.zip?ts={Uri.EscapeDataString(DateTime.UtcNow.Ticks.ToString())}", cancellationToken).ConfigureAwait(false);
        var downloadPath = Path.Combine(targetDirectoryToExtract, "Images.zip");
        using var localStream = File.Open(downloadPath, FileMode.Create, FileAccess.ReadWrite);
        await contentStream.CopyToAsync(localStream, cancellationToken).ConfigureAwait(false);
        localStream.Seek(0L, SeekOrigin.Begin);

        targetDirectoryToExtract = Directory.CreateDirectory(targetDirectoryToExtract).FullName;
        await _archiveExpander.ExpandArchiveAsync(localStream, targetDirectoryToExtract, cancellationToken).ConfigureAwait(false);
    }

    public string GetLocalImagePath(string imageDirectory, string serviceId)
        => Path.GetFullPath(Path.Combine(imageDirectory, serviceId + ".png"));

    public string GetLocalIconPath(string imageDirectory, string serviceId)
        => Path.GetFullPath(Path.Combine(imageDirectory, serviceId + ".ico"));
}
