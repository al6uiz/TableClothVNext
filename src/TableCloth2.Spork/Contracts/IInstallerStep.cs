namespace TableCloth2.Spork.Contracts;

public interface IInstallerStep
{
    string StepName { get; }

    Task<Exception?> PerformStepAsync(CancellationToken cancellationToken = default);
}
