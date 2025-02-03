using TableCloth2.Spork.Contracts;

namespace TableCloth2.Spork.Steps;

internal sealed class NoOpStep : IInstallerStep
{
    internal static readonly NoOpStep Instance = new();

    private NoOpStep() { }

    public string StepName => string.Empty;

    public Task<Exception?> PerformStepAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<Exception?>(null);
}
