namespace TableCloth3.Spork.Services;

public sealed class SporkLocationService
{
    public string AppDataDirectory
        => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Spork");

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
}
