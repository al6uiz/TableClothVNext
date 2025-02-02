using System.Windows.Input;
using TableCloth2.Shared;

namespace TableCloth2.TableCloth.ViewModels;

public sealed class SettingsViewModel : ViewModelBase
{
    public SettingsViewModel()
    {
        _includeFolderCommand = new RelayCommand(IncludeFolder);
        _excludeFolderCommand = new RelayCommand(ExcludeFolder);
        _excludeAllFoldersCommand = new RelayCommand(ExcludeAllFolders);
    }

    private readonly RelayCommand _includeFolderCommand;
    private readonly RelayCommand _excludeFolderCommand;
    private readonly RelayCommand _excludeAllFoldersCommand;

    public ICommand IncludeFolderCommand => _includeFolderCommand;
    public ICommand ExcludeFolderCommand => _excludeFolderCommand;
    public ICommand ExcludeAllFoldersCommand => _excludeAllFoldersCommand;

    private void IncludeFolder(object? _)
    {
        var selectedPaths = new List<string>();
        var args = new RelayEventArgs<List<string>>(selectedPaths);
        FolderSelect?.Invoke(this, args);

        if (!args.Accepted)
            return;

        var modifiedList = new List<string>(_folderMountList);

        foreach (var eachSelectedPath in selectedPaths)
            modifiedList.Add(eachSelectedPath);

        FolderMountList = modifiedList;
    }

    private void ExcludeFolder(object? _)
    {
        if (string.IsNullOrWhiteSpace(_selectedFolderMountPath))
            return;

        var foundIndex = _folderMountList
            .FindIndex(x => string.Equals(x, _selectedFolderMountPath, StringComparison.OrdinalIgnoreCase));

        if (foundIndex > (-1))
        {
            var modifiedList = new List<string>(_folderMountList);
            modifiedList.RemoveAt(foundIndex);
            FolderMountList = modifiedList;
        }
    }

    private void ExcludeAllFolders(object? _)
    {
        var modifiedList = new List<string>(_folderMountList);
        modifiedList.Clear();
        FolderMountList = modifiedList;
        SelectedFolderMountPath = string.Empty;
    }

    private bool _mountNPKICerts;
    private bool _enableFolderMount;
    private List<string> _folderMountList = new List<string>();
    private string? _selectedFolderMountPath;
    private bool _enableAudioInput;
    private bool _enableVideoInput;
    private bool _enablePrinterRedirection;
    private bool _enableVirtualizedGpu;
    private bool _useCloudflareDns;
    private bool _collectSentryLog;
    private bool _collectAnalytics;

    public bool MountNPKICerts
    {
        get => _mountNPKICerts;
        set => SetField(ref _mountNPKICerts, value);
    }

    public bool EnableFolderMount
    {
        get => _enableFolderMount;
        set => SetField(ref _enableFolderMount, value);
    }

    public List<string> FolderMountList
    {
        get => _folderMountList;
        set => SetField(ref _folderMountList, value);
    }

    public string? SelectedFolderMountPath
    {
        get => _selectedFolderMountPath;
        set => SetField(ref _selectedFolderMountPath, value);
    }

    public bool EnableAudioInput
    {
        get => _enableAudioInput;
        set => SetField(ref _enableAudioInput, value);
    }

    public bool EnableVideoInput
    {
        get => _enableVideoInput;
        set => SetField(ref _enableVideoInput, value);
    }

    public bool EnablePrinterRedirection
    {
        get => _enablePrinterRedirection;
        set => SetField(ref _enablePrinterRedirection, value);
    }

    public bool EnableVirtualizedGpu
    {
        get => _enableVirtualizedGpu;
        set => SetField(ref _enableVirtualizedGpu, value);
    }

    public bool UseCloudflareDns
    {
        get => _useCloudflareDns;
        set => SetField(ref _useCloudflareDns, value);
    }

    public bool CollectSentryLog
    {
        get => _collectSentryLog;
        set => SetField(ref _collectSentryLog, value);
    }

    public bool CollectAnalytics
    {
        get => _collectAnalytics;
        set => SetField(ref _collectAnalytics, value);
    }

    public event EventHandler<RelayEventArgs<List<string>>>? FolderSelect;
}
