using TableCloth2.Shared.Models;

namespace TableCloth2.Shared.Contracts;

public interface IBootstrapper
{
    Task<BootstrapResult> PerformBootstrapAsync(CancellationToken cancellationToken = default);
}
