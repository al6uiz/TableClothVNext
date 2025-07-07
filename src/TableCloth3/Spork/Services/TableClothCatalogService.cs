using Avalonia.Xaml.Interactions.Custom;
using System.Xml.Linq;
using TableCloth3.Shared;
using TableCloth3.Shared.Languages;
using TableCloth3.Spork.Languages;

namespace TableCloth3.Spork.Services;

public sealed class TableClothCatalogService
{
    public TableClothCatalogService(
        IHttpClientFactory httpClientFactory)
        : base()
    {
        _httpClientFactory = httpClientFactory;
    }

    private readonly IHttpClientFactory _httpClientFactory = default!;

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
}
