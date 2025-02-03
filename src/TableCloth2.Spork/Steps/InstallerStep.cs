using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Contracts;

namespace TableCloth2.Spork.Steps;

public sealed class InstallerStep : IInstallerStep
{
    public InstallerStep(CatalogPackageInformation package)
    {
        _package = package ?? throw new ArgumentNullException(nameof(package));
    }

    private readonly CatalogPackageInformation _package;

    public string StepName => $"Installing '{_package.Name}...'";

    public async Task<Exception?> PerformStepAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(1.5d), cancellationToken);
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
