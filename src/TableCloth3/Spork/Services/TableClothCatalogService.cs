using System.Xml.Linq;

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
}
