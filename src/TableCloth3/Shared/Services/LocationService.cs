namespace TableCloth3.Shared.Services;

public sealed class LocationService
{
    public LocationService(
        ScenarioRouter scenarioRouter)
        : base()
    {
        _scenarioRouter = scenarioRouter;
    }

    private readonly ScenarioRouter _scenarioRouter;

    public string AppDataDirectory
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), _scenarioRouter.GetScenario().ToString());

    public DirectoryInfo EnsureAppDataDirectoryCreated()
        => Directory.CreateDirectory(AppDataDirectory);

    public string ImagesDirectory
        => Path.Combine(AppDataDirectory, "Images");

    public DirectoryInfo EnsureImagesDirectoryCreated()
        => Directory.CreateDirectory(ImagesDirectory);

    public string DownloadsDirectory
        => Path.Combine(AppDataDirectory, "Downloads");

    public DirectoryInfo EnsureDownloadsDirectoryCreated()
        => Directory.CreateDirectory(DownloadsDirectory);

    public string WindowsSandboxProfilePath
        => Path.Combine(AppDataDirectory, "Sandbox.wsb");

    public string WindowsSandboxLauncherPath
        => Path.Combine(AppDataDirectory, "Launch.ps1");

    public string DesktopDirectory
        => Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    public DirectoryInfo EnsureDesktopDirectoryCreated()
        => Directory.CreateDirectory(DesktopDirectory);

    public string GetDesktopSubDirectory(string directoryName)
    {
        if (string.IsNullOrWhiteSpace(directoryName))
            throw new ArgumentException("Directory name cannot be null or whitespace.", nameof(directoryName));
        return Path.Combine(DesktopDirectory, directoryName);
    }

    public DirectoryInfo GetDesktopSubDirectoryInfo(string directoryName)
        => new DirectoryInfo(GetDesktopSubDirectory(directoryName));

    public string LocalLowAppDataDirectory
        => SafeNativeMethods.GetLocalAppDataLowFolder();

    public DirectoryInfo EnsureLocalLowAppDataDirectoryCreated()
        => Directory.CreateDirectory(LocalLowAppDataDirectory);

    public string LocalLowNpkiDirectory
        => Path.Combine(LocalLowAppDataDirectory, "NPKI");

    public DirectoryInfo EnsureLocalLowNpkiDirectoryCreated()
        => Directory.CreateDirectory(LocalLowNpkiDirectory);
}
