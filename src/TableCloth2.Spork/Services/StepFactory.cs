using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Contracts;
using TableCloth2.Spork.Steps;

namespace TableCloth2.Spork.Services;

public sealed class StepFactory
{
    public StepFactory(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private readonly IHttpClientFactory _httpClientFactory;

    public IInstallerStep CreateDownloadStep(CatalogPackageInformation package)
        => new DownloadStep(package, _httpClientFactory);

    public IInstallerStep CreateInstallerStep(CatalogPackageInformation package)
        => new InstallerStep(package);
}
