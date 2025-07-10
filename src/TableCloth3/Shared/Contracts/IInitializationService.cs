namespace TableCloth3.Shared.Contracts;

public interface IInitializationService
{
    Task InitializeAsync(string[] args, CancellationToken cancellationToken = default);
}
