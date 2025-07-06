using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Net.Http;
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
        _categoryItems = Enum.GetValues<TableClothItemCategory>()
            .Select(x => new KeyValuePair<TableClothItemCategory, string>(x, x.ToString()))
            .ToArray();

        Items.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasNoItems));
            OnPropertyChanged(nameof(FilteredItems));
        };

        ApplyFilter();
    }
    
    public sealed record class AboutButtonRequest;

    public interface IAboutButtonRequestRecipient : IRecipient<AboutButtonRequest>;

    public sealed record class CloseButtonRequest;

    public interface ICloseButtonRequestRecipient : IRecipient<CloseButtonRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly TableClothCatalogService _catalogService = default!;
    private readonly KeyValuePair<TableClothItemCategory, string>[] _categoryItems = [];

    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            Items.Add(new()
            {
                Category = TableClothItemCategory.Financing,
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

    public bool HasItems => FilteredItems.Any();

    public bool HasNoItems => !FilteredItems.Any();

    public IReadOnlyList<KeyValuePair<TableClothItemCategory, string>> CategoryItems => _categoryItems;

    partial void OnFilterTextChanged(string value)
        => ApplyFilter();

    [RelayCommand]
    private void ApplyFilter()
    {
        var query = (IEnumerable<TableClothCatalogItemViewModel>)Items;
        if (!string.IsNullOrWhiteSpace(FilterText))
            query = query.Where(x => x.DisplayName.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        query = query.OrderBy(x => x.DisplayName);
        FilteredItems = query;
    }

    [RelayCommand]
    private void AboutButton()
        => _messenger.Send<AboutButtonRequest>();

    [RelayCommand]
    private async Task RefreshCatalog(CancellationToken cancellationToken = default)
    {
        Items.Clear();

        var doc = await _catalogService.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
        var services = doc.XPathSelectElements("/TableClothCatalog/InternetServices/Service");

        foreach (var eachService in services)
        {
            var id = eachService.Attribute("Id")?.Value;

            if (string.IsNullOrWhiteSpace(id))
                continue;

            Enum.TryParse<TableClothItemCategory>(
                eachService.Attribute("Category")?.Value, true, out var category);
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

        ApplyFilter();
    }

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonRequest>();
}

public sealed class TableClothCategoryFilterItem
{
    public string Name { get; set; }
}

public sealed partial class TableClothCatalogItemViewModel : BaseViewModel
{
    [ObservableProperty]
    private string _serviceId = string.Empty;

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private TableClothItemCategory _category;

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

public enum TableClothItemCategory
{
    None,
    Banking,
    Financing,
    Security,
    CreditCard,
    Insurance,
    Government,
    Education,
    Other,
}
