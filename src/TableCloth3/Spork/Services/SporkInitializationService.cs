using TableCloth3.Shared.Contracts;

namespace TableCloth3.Spork.Services;

internal sealed class SporkInitializationService : IInitializationService
{
    public async Task InitializeAsync(string[] args, CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(3d), cancellationToken).ConfigureAwait(false);
    }
}
