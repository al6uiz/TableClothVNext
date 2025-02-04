using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Contracts;

namespace TableCloth2.Spork.Steps;

public sealed class DownloadStep : IInstallerStep
{
    public DownloadStep(
        CatalogPackageInformation package,
        IHttpClientFactory httpClientFactory)
    {
        _package = package ?? throw new ArgumentNullException(nameof(package));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
    }

    private readonly CatalogPackageInformation _package;
    private readonly IHttpClientFactory _httpClientFactory;

    public string StepName => $"Downloading '{_package.Name}...'";

    public async Task<Exception?> PerformStepAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var chromeLikeClient = _httpClientFactory.GetChromeLikeHttpClient();
            using var remoteStream = await chromeLikeClient.GetStreamAsync(_package.Url, cancellationToken);
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
