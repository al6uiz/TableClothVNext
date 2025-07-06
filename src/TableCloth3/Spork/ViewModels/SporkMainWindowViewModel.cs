using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Xml.XPath;
using TableCloth3.Shared.ViewModels;
using TableCloth3.Spork.Services;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class SporkMainWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindowViewModel(
        IMessenger messenger,
        TableClothCatalogService catalogService)
        : this()
    {
        _messenger = messenger;
        _catalogService = catalogService;
    }

    public SporkMainWindowViewModel()
        : base()
    {
        Items.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasNoItems));
            OnPropertyChanged(nameof(FilteredItems));
        };

        ApplyFilter();
    }

    public sealed record class LoadingFailureNotification(Exception OccurredException);

    public interface ILoadingFailureNotificationRecipient : IRecipient<LoadingFailureNotification>;
    
    public sealed record class AboutButtonRequest;

    public interface IAboutButtonRequestRecipient : IRecipient<AboutButtonRequest>;

    public sealed record class CloseButtonRequest;

    public interface ICloseButtonRequestRecipient : IRecipient<CloseButtonRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly TableClothCatalogService _catalogService = default!;

    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            Items.Add(new()
            {
                Category = "Financing",
                DisplayName = $"Test {i + 1}",
                ServiceId = $"test{i + 1}",
                TargetUrl = "https://yourtablecloth.app",
                Packages = new()
                {
                    new() { PackageName = "Test Package", PackageArguments = "/S", PackageUrl = "https://www.google.com/", },
                },
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<TableClothCatalogItemViewModel> _items = [];

    [ObservableProperty]
    private IEnumerable<TableClothCatalogItemViewModel> _filteredItems = [];

    [ObservableProperty]
    private string _filterText = string.Empty;

    [ObservableProperty]
    private TableClothCategoryItemViewModel? _selectedCategory = default;

    [ObservableProperty]
    private ObservableCollection<TableClothCategoryItemViewModel> _categoryItems = [];

    [ObservableProperty]
    private bool _isLoading = false;

    public bool HasItems
    {
        get
        {
            if (IsLoading) return false;

            try { return FilteredItems.Any(); }
            catch { return false; }
        }
    }

    public bool HasNoItems => !IsLoading && !HasItems;

    partial void OnFilterTextChanged(string value)
        => ApplyFilter();

    partial void OnSelectedCategoryChanged(TableClothCategoryItemViewModel? value)
        => ApplyFilter();

    [RelayCommand]
    private void Loaded()
        => RefreshCatalog().SafeFireAndForget();

    [RelayCommand]
    private void ApplyFilter()
    {
        var query = (IEnumerable<TableClothCatalogItemViewModel>)Items;
        if (!string.IsNullOrWhiteSpace(FilterText))
            query = query.Where(x => x.DisplayName.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        if (SelectedCategory != null && !SelectedCategory.IsWildcard)
            query = query.Where(x => x.Category.Equals(SelectedCategory.CategoryName, StringComparison.OrdinalIgnoreCase));
        FilteredItems = query;
    }

    [RelayCommand]
    private void AboutButton()
        => _messenger.Send<AboutButtonRequest>();

    [RelayCommand]
    private async Task RefreshCatalog(CancellationToken cancellationToken = default)
    {
        try
        {
            IsLoading = true;
            Items.Clear();

            var doc = await _catalogService.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
            var services = doc.XPathSelectElements("/TableClothCatalog/InternetServices/Service");

            foreach (var eachService in services)
            {
                var id = eachService.Attribute("Id")?.Value;

                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var category = eachService.Attribute("Category")?.Value ?? "Unknown";
                var displayName = eachService.Attribute("DisplayName")?.Value;
                var url = eachService.Attribute("Url")?.Value;

                Items.Add(new()
                {
                    ServiceId = id,
                    DisplayName = displayName ?? "(Unknown)",
                    Category = category,
                    TargetUrl = url ?? string.Empty,
                });
            }
        }
        catch (Exception ex)
        {
            _messenger.Send<LoadingFailureNotification>(new(ex));
        }
        finally
        {
            IsLoading = false;
        }

        CategoryItems.Clear();

        var allItemCategory = new TableClothCategoryItemViewModel()
        {
            CategoryName = "All",
            IsWildcard = true,
        };
        CategoryItems.Add(allItemCategory);
        SelectedCategory = allItemCategory;
        foreach (var eachCategory in Items.Select(x => x.Category).Distinct())
        {
            CategoryItems.Add(new TableClothCategoryItemViewModel()
            {
                CategoryName = eachCategory,
                IsWildcard = false,
            });
        }

        ApplyFilter();
    }

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonRequest>();
}

public sealed partial class TableClothCatalogItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _serviceId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _targetUrl = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TableClothPackageItemViewModel> _packages = new();

    [RelayCommand]
    private void LaunchSite()
    {
    }
}

public sealed partial class TableClothPackageItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _packageName = string.Empty;

    [ObservableProperty]
    private string _packageUrl = string.Empty;

    [ObservableProperty]
    private string _packageArguments = string.Empty;
}

public sealed partial class TableClothCategoryItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _categoryName = string.Empty;

    [ObservableProperty]
    private bool _isWildcard = false;

    public string CategoryDisplayName
    {
        get { return CategoryName; }
    }
}
