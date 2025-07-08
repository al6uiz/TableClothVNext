namespace TableCloth3.Spork.Services;

public sealed class SporkLocationService
{
    public string AppDataDirectory
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spork");

    public DirectoryInfo EnsureAppDataDirectoryCreated()
        => Directory.CreateDirectory(AppDataDirectory);

    public string ImagesDirectory
        => Path.Combine(AppDataDirectory);

    public DirectoryInfo EnsureImagesDirectoryCreated()
        => Directory.CreateDirectory(ImagesDirectory);

    public string TemporaryDirectory
        => Path.GetTempPath();
}
