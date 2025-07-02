using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Net.Http;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class SporkMainWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindowViewModel(
        IMessenger messenger,
        IHttpClientFactory httpClientFactory)
        : this()
    {
        _messenger = messenger;
        _httpClientFactory = httpClientFactory;
    }

    public SporkMainWindowViewModel()
        : base()
    {
        Items.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasNoItems));
        };

        ApplyFilter();
    }
    
    public sealed record class AboutButtonRequest;

    public interface IAboutButtonRequestRecipient : IRecipient<AboutButtonRequest>;

    public sealed record class CloseButtonRequest;

    public interface ICloseButtonRequestRecipient : IRecipient<CloseButtonRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly IHttpClientFactory _httpClientFactory = default!;

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
    private ObservableCollection<TableClothCatalogItemViewModel> _filteredItems = [];

    [ObservableProperty]
    private string _filterText = string.Empty;

    public bool HasItems => FilteredItems.Any();

    public bool HasNoItems => !FilteredItems.Any();

    partial void OnFilterTextChanged(string? oldValue, string newValue)
        => ApplyFilter();

    [RelayCommand]
    private void ApplyFilter()
    {
        var query = (IEnumerable<TableClothCatalogItemViewModel>)Items;
        if (!string.IsNullOrWhiteSpace(FilterText))
            query = query.Where(x => x.DisplayName.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        query = query.OrderBy(x => x.DisplayName);

        FilteredItems.Clear();
        foreach (var eachFilteredItem in query)
            FilteredItems.Add(eachFilteredItem);
    }

    [RelayCommand]
    private void AboutButton()
        => _messenger.Send<AboutButtonRequest>();

    [RelayCommand]
    private async Task RefreshCatalog(CancellationToken cancellationToken = default)
    {
        var httpClient = _httpClientFactory.CreateCatalogHttpClient();
        var content = await httpClient.GetStringAsync($"/TableClothCatalog/Catalog.xml?ts={Uri.EscapeDataString(DateTime.UtcNow.Ticks.ToString())}", cancellationToken).ConfigureAwait(false);
        return;
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
