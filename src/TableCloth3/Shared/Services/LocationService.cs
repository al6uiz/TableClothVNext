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
}
