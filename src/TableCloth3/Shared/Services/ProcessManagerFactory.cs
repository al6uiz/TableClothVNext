using TableCloth3.Shared.Contracts;

namespace TableCloth3.Shared.Services;

public sealed class ProcessManagerFactory : IProcessManagerFactory
{
    public IProcessManager Create()
    {
        return new ProcessManager();
    }
}
