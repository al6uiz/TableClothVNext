using TableCloth3.Shared.Contracts;

namespace TableCloth3.Shared.Services;

internal sealed class EmptyInitializationService : IInitializationService
{
    public Task InitializeAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
