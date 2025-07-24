using TableCloth3.Spork.Contracts;

namespace TableCloth3.Spork.Services;

public sealed class ProcessManagerFactory : IProcessManagerFactory
{
    public IProcessManager Create()
    {
        return new ProcessManager();
    }
}
