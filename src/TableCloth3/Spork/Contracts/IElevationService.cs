namespace TableCloth3.Spork.Contracts;

public interface IElevationService
{
    bool IsElevated();
    void RestartAsElevated(string[] args);
}
