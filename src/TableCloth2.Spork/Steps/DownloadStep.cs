using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Contracts;

namespace TableCloth2.Spork.Steps;

public sealed class DownloadStep : IInstallerStep
{
    public DownloadStep(CatalogPackageInformation package)
    {
        _package = package ?? throw new ArgumentNullException(nameof(package));
    }

    private readonly CatalogPackageInformation _package;

    public string StepName => $"Downloading '{_package.Name}...'";

    public async Task<Exception?> PerformStepAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1d), cancellationToken);
            return null;
        }
        catch (OperationCanceledException)
        {
            return new OperationCanceledException();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
