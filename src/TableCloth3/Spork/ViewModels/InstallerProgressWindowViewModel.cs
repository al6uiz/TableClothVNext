using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerProgressWindowViewModel : BaseViewModel
{
    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];
}

public sealed partial class InstallerStepItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _packageName = string.Empty;

    [ObservableProperty]
    private string _packageUrl = string.Empty;

    [ObservableProperty]
    private string _packageArguments = string.Empty;
}
