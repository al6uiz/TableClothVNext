using TableCloth3.Shared.Contracts;

namespace TableCloth3.Launcher.Services;

internal sealed class LauncherInitializationService : IInitializationService
{
    public Task InitializeAsync(string[] args, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
