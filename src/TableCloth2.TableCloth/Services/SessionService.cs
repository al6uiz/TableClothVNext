using TableCloth2.Services;

namespace TableCloth2.TableCloth.Services;

public sealed class SessionService : IDisposable
{
    public SessionService(
        KnownPathsService knownPathServices)
    {
        _knownPathServices = knownPathServices;
        _sessionId = Guid.NewGuid();
        _mutex = new Mutex(true, "Global\\TableCloth2", out _isNewInstance);
    }

    private bool _disposed;
    private readonly bool _isNewInstance;
    private readonly Guid _sessionId;
    private readonly Mutex _mutex;
    private readonly KnownPathsService _knownPathServices;

    public Guid SessionId => _sessionId;

    public bool IsNewInstance => _isNewInstance;

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _mutex.Dispose();
            }

            _disposed = true;
        }
    }

    ~SessionService() => Dispose(false);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public DirectoryInfo CreateSessionDirectory()
        => _knownPathServices.EnsureTableClothSessionDirectoryExists(_sessionId);
}
