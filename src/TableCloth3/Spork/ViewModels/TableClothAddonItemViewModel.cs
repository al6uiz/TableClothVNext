using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class TableClothAddonItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _addonId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _targetUrl = string.Empty;

    [ObservableProperty]
    private string _arguments = string.Empty;

    [ObservableProperty]
    private Bitmap? _addonIcon = null;

    [RelayCommand]
    private void LaunchAddon()
    {
        // TODO: Implement addon installation logic
    }
}
