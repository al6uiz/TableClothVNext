namespace TableCloth3.Launcher.Models;

public sealed class LauncherSettingsModel
{
    public bool UseMicrophone { get; set; }
    public bool UseWebCamera { get; set; }
    public bool SharePrinters { get; set; }
    public bool MountNpkiFolders { get; set; }
    public bool MountSpecificFolders { get; set; }
    public string[] Folders { get; set; } = Array.Empty<string>();
}
