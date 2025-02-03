using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Contracts;
using TableCloth2.Spork.Steps;

namespace TableCloth2.Spork.Services;

public sealed class StepFactory
{
    public IInstallerStep CreateDownloadStep(CatalogPackageInformation package)
        => new DownloadStep(package);

    public IInstallerStep CreateInstallerStep(CatalogPackageInformation package)
        => new InstallerStep(package);
}
