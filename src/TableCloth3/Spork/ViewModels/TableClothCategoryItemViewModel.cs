using CommunityToolkit.Mvvm.ComponentModel;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class TableClothCategoryItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private string _categoryDisplayName = string.Empty;

    [ObservableProperty]
    private bool _isWildcard = false;
}
