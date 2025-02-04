using TableCloth2.Shared;

namespace TableCloth2.Models;

public sealed class SettingsModel
{
    public bool MountNPKICerts { get; set; } = true;
    public bool EnableFolderMount { get; set; } = true;
    public ObservableListSource<string> FolderMountList { get; set; } = new ObservableListSource<string>();

    public bool EnableAudioInput { get; set; } = false;
    public bool EnableVideoInput { get; set; } = false;
    public bool EnablePrinterRedirection { get; set; } = false;
    public bool EnableVirtualizedGpu { get; set; } = false;
    public bool UseCloudflareDns { get; set; } = false;

    public bool CollectSentryLog { get; set; } = true;
    public bool CollectAnalytics { get; set; } = true;
}
