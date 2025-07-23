namespace TableCloth3.Spork.Contracts;

public interface ITaskBarProgressService
{
    public ITaskBarProgressHost? ProgressHost { get; set; }

    public void Reset() => ProgressHost?.Reset();

    void SetProgress(bool hasError, ulong currentValue, ulong totalValue) => ProgressHost?.SetProgress(hasError, currentValue, totalValue);
}

public interface ITaskBarProgressHost
{
    void Reset();

    void SetProgress(bool hasError, ulong currentValue, ulong totalValue);
}