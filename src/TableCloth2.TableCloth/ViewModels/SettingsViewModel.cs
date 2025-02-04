using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Shared;

namespace TableCloth2.TableCloth.ViewModels;

public sealed partial class SettingsViewModel : ObservableObject
{
    public SettingsViewModel(
        IMessenger messenger)
    {
        _messenger = messenger;
    }

    private readonly IMessenger _messenger;

    [ObservableProperty]
    private bool mountNPKICerts;

    [ObservableProperty]
    private bool enableFolderMount;

    [ObservableProperty]
    private ObservableListSource<string> folderMountList = new ObservableListSource<string>();

    [ObservableProperty]
    private string? selectedFolderMountPath;

    [ObservableProperty]
    private bool enableAudioInput;

    [ObservableProperty]
    private bool enableVideoInput;

    [ObservableProperty]
    private bool enablePrinterRedirection;

    [ObservableProperty]
    private bool enableVirtualizedGpu;

    [ObservableProperty]
    private bool useCloudflareDns;

    [ObservableProperty]
    private bool collectSentryLog;

    [ObservableProperty]
    private bool collectAnalytics;

    [RelayCommand]
    private async Task IncludeFolder()
    {
        var selectedPaths = await _messenger.Send<AsyncRequestMessage<IEnumerable<string>>, int>((int)Messages.FolderSelect);

        if (!selectedPaths.Any())
            return;

        foreach (var eachSelectedPath in selectedPaths)
            FolderMountList.Add(eachSelectedPath);
    }

    [RelayCommand]
    private void ExcludeFolder()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolderMountPath))
            return;

        var foundIndex = (-1);
        for (var i = 0; i < FolderMountList.Count; i++)
        {
            if (string.Equals(FolderMountList[i], SelectedFolderMountPath, StringComparison.OrdinalIgnoreCase))
            {
                foundIndex = i;
                break;
            }
        }

        if (foundIndex > (-1))
            FolderMountList.RemoveAt(foundIndex);
    }

    [RelayCommand]
    private void ExcludeAllFolders()
    {
        FolderMountList.Clear();
        SelectedFolderMountPath = string.Empty;
    }
}
