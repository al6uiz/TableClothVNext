namespace TableCloth2.Services;

public sealed class KnownPathsService
{
    private readonly DirectoryInfo _userProfileDirectory =
        new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

    private readonly DirectoryInfo _appDataDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData"));

    private readonly DirectoryInfo _localAppDataDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local"));

    private readonly DirectoryInfo _localLowAppDataDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow"));

    private readonly DirectoryInfo _localLowNPKIDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "LocalLow", "NPKI"));

    private readonly DirectoryInfo _tableclothHomeDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "TableCloth2"));

    private readonly DirectoryInfo _tableclothSettingsDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "TableCloth2", "Settings"));

    private readonly DirectoryInfo _tableclothLogsDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "TableCloth2", "Logs"));

    private readonly DirectoryInfo _tableclothSessionDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "TableCloth2", "Sessions"));

    private readonly DirectoryInfo _sandboxDesktopDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop"));

    private readonly DirectoryInfo _sandboxDesktopNPKIDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "NPKI"));

    private readonly DirectoryInfo _sandboxDesktopSettingsDirectory =
        new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Desktop", "Settings"));

    public DirectoryInfo UserProfileDirectory => _userProfileDirectory;
    public DirectoryInfo AppDataDirectory => _appDataDirectory;
    public DirectoryInfo LocalAppDataDirectory => _localAppDataDirectory;
    public DirectoryInfo LocalLowAppDataDirectory => _localLowAppDataDirectory;
    public DirectoryInfo LocalLowNPKIDirectory => _localLowNPKIDirectory;
    public DirectoryInfo TableClothHomeDirectory => _tableclothHomeDirectory;
    public DirectoryInfo TableClothSettingsDirectory => _tableclothSettingsDirectory;
    public DirectoryInfo TableClothLogsDirectory => _tableclothLogsDirectory;
    public DirectoryInfo TableClothSessionDirectory => _tableclothSessionDirectory;
    public DirectoryInfo SandboxDesktopDirectory => _sandboxDesktopDirectory;
    public DirectoryInfo SandboxDesktopNPKIDirectory => _sandboxDesktopNPKIDirectory;
    public DirectoryInfo SandboxDesktopSettingsDirectory => _sandboxDesktopSettingsDirectory;

    public DirectoryInfo EnsureLocalLowNPKIDirectoryExists()
    {
        _localLowNPKIDirectory.Create();
        return _localLowNPKIDirectory;
    }

    public DirectoryInfo EnsureTableClothHomeDirectoryExists()
    {
        _tableclothHomeDirectory.Create();
        return _tableclothHomeDirectory;
    }

    public DirectoryInfo EnsureTableClothSettingsDirectoryExists()
    {
        _tableclothSettingsDirectory.Create();
        return _tableclothSettingsDirectory;
    }

    public DirectoryInfo EnsureTableClothLogsDirectoryExists()
    {
        _tableclothLogsDirectory.Create();
        return _tableclothLogsDirectory;
    }

    public DirectoryInfo EnsureTableClothSessionDirectoryExists(Guid? sessionId)
    {
        if (!sessionId.HasValue)
            return _tableclothSessionDirectory;

        var fullPath = _tableclothSessionDirectory.FullName;
        fullPath = Path.Combine(fullPath, sessionId.Value.ToString("n"));

        var directoryInfo = new DirectoryInfo(fullPath);
        directoryInfo.Create();
        return directoryInfo;
    }
}
