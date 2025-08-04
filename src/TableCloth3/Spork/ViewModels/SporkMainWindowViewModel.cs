using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Xml.XPath;
using TableCloth3.Shared.Languages;
using TableCloth3.Shared.Models;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class SporkMainWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindowViewModel(
        IMessenger messenger,
        TableClothCatalogService catalogService,
        AvaloniaViewModelManager avaloniaViewModelManager,
        LocationService sporkLocationService,
        ScenarioRouter scenarioRouter)
        : this()
    {
        _messenger = messenger;
        _catalogService = catalogService;
        _avaloniaViewModelManager = avaloniaViewModelManager;
        _sporkLocationService = sporkLocationService;
        _scenarioRouter = scenarioRouter;
    }

    public SporkMainWindowViewModel()
        : base()
    {
        Items.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasNoItems));
            OnPropertyChanged(nameof(IsLoading));
            OnPropertyChanged(nameof(FilteredItems));
        };

        ApplyFilter();
    }

    partial void OnIsLoadingChanged(bool value)
        => OnPropertyChanged(nameof(IsLoadingCompleted));

    public sealed record class LoadingFailureNotification(Exception OccurredException);

    public interface ILoadingFailureNotificationRecipient : IRecipient<LoadingFailureNotification>;
    
    public sealed record class AboutButtonRequest;

    public interface IAboutButtonRequestRecipient : IRecipient<AboutButtonRequest>;

    public sealed record class CloseButtonRequest;

    public interface ICloseButtonRequestRecipient : IRecipient<CloseButtonRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly TableClothCatalogService _catalogService = default!;
    private readonly AvaloniaViewModelManager _avaloniaViewModelManager = default!;
    private readonly LocationService _sporkLocationService = default!;
    private readonly ScenarioRouter _scenarioRouter = default!;

    protected override void PrepareDesignTimePreview()
    {
        CategoryItems.Add(new() { CategoryName = "Financing", IsWildcard = false, });

        for (var i = 0; i < 100; i++)
        {
            Items.Add(new(default!, default!, default!)
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

    public bool IsLoadingCompleted => !IsLoading;

    public bool HasItems
    {
        get
        {
            try { return FilteredItems.Any(); }
            catch { return false; }
        }
    }

    public bool HasNoItems => !HasItems;

    partial void OnFilterTextChanged(string value)
        => ApplyFilter();

    partial void OnSelectedCategoryChanged(TableClothCategoryItemViewModel? value)
        => ApplyFilter();

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken = default)
    {
        if (Design.IsDesignMode)
            return;

        await RefreshCatalog(cancellationToken).ConfigureAwait(false);
    }

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

            if (_scenarioRouter.GetSporkScenario() == SporkScenario.Standalone)
            {
                var catalogDownloadTask = _catalogService.DownloadCatalogAsync(cancellationToken);
                var imageDownloadTask = _catalogService.DownloadImagesAsync(cancellationToken);
                await Task.WhenAll(catalogDownloadTask, imageDownloadTask).ConfigureAwait(false);
            }
            else
            {
                var destAppDataDirectory = _sporkLocationService.EnsureAppDataDirectoryCreated().FullName;
                var destImagesDirectory = _sporkLocationService.EnsureImagesDirectoryCreated().FullName;

                var launcherDataDirectory = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "Launcher");
                var launcherImagesDirectory = Path.Combine(launcherDataDirectory, "Images");

                File.Copy(
                    Path.Combine(launcherDataDirectory, "build-info.json"),
                    Path.Combine(destAppDataDirectory, "build-info.json"),
                    true);
                File.Copy(
                    Path.Combine(launcherDataDirectory, "Catalog.xml"),
                    Path.Combine(destAppDataDirectory, "Catalog.xml"),
                    true);

                foreach (var eachFile in Directory.GetFiles(launcherImagesDirectory, "*.*"))
                {
                    File.Copy(
                        eachFile,
                        Path.Combine(destImagesDirectory, Path.GetFileName(eachFile)),
                        true);
                }
            }

            var doc = await _catalogService.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
            var services = doc.XPathSelectElements("/TableClothCatalog/InternetServices/Service");

            foreach (var eachService in services)
            {
                var id = eachService.Attribute("Id")?.Value;

                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var category = eachService.Attribute("Category")?.Value ?? string.Empty;
                var displayName = eachService.Attribute("DisplayName")?.Value ?? id;
                var url = eachService.Attribute("Url")?.Value;

                var viewModel = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothCatalogItemViewModel>();
                viewModel.ServiceId = id;
                viewModel.DisplayName = displayName;
                viewModel.Category = category;
                viewModel.CategoryDisplayName = _catalogService.GetCatalogDisplayName(category);
                viewModel.TargetUrl = url ?? string.Empty;

                foreach (var eachPackage in eachService?.Element("Packages")?.Elements("Package") ?? Array.Empty<XElement>())
                {
                    var packageUrl = eachPackage.Attribute("Url")?.Value ?? string.Empty;

                    if (string.IsNullOrWhiteSpace(packageUrl))
                        continue;

                    var packageName = eachPackage.Attribute("Name")?.Value ?? "UnknownPackage";
                    var packageArgs = eachPackage.Attribute("Arguments")?.Value ?? string.Empty;

                    var packageViewModel = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothPackageItemViewModel>();
                    packageViewModel.PackageUrl = packageUrl;
                    packageViewModel.PackageName = packageName;
                    packageViewModel.PackageArguments = packageArgs;
                    viewModel.Packages.Add(packageViewModel);
                }

                Items.Add(viewModel);
            }
        }
        catch (Exception ex)
        {
            _messenger?.Send<LoadingFailureNotification>(new(ex));
        }
        finally
        {
            IsLoading = false;
        }

        CategoryItems.Clear();

        var allItemCategory = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothCategoryItemViewModel>();
        allItemCategory.CategoryName = "All";
        allItemCategory.CategoryDisplayName = SharedStrings.AllCategoryDisplayName;
        allItemCategory.IsWildcard = true;

        CategoryItems.Add(allItemCategory);
        SelectedCategory = allItemCategory;
        foreach (var eachCategory in Items.Select(x => x.Category).Distinct())
        {
            var eachItem = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothCategoryItemViewModel>();
            eachItem.CategoryName = eachCategory;
            eachItem.CategoryDisplayName = _catalogService.GetCatalogDisplayName(eachCategory);
            eachItem.IsWildcard = false;
            CategoryItems.Add(eachItem);
        }

        ApplyFilter();
    }

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonRequest>();
}

public sealed partial class TableClothCatalogItemViewModel : BaseViewModel
{
    public TableClothCatalogItemViewModel(
        IMessenger messenger,
        LocationService sporkLocationService,
        TableClothCatalogService catalogService)
        : base()
    {
        _messenger = messenger;
        _sporkLocationService = sporkLocationService;
        _catalogService = catalogService;
    }

    public sealed record class LaunchSiteRequest(TableClothCatalogItemViewModel ViewModel);

    public interface ILaunchSiteRequestRecipient : IRecipient<LaunchSiteRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly LocationService _sporkLocationService = default!;
    private readonly TableClothCatalogService _catalogService = default!;

    partial void OnServiceIdChanged(string value)
    {
        if (Design.IsDesignMode)
            return;

        var targetPath = _catalogService.GetLocalImagePath(value);

        if (File.Exists(targetPath))
            ServiceIcon = new Bitmap(targetPath);
        else
            ServiceIcon = new Bitmap(AssetLoader.Open(new Uri("avares://TableCloth3/Assets/Images/Spork.png")));
    }

    [ObservableProperty]
    private string _serviceId = string.Empty;

    [ObservableProperty]
    private Bitmap? _serviceIcon = new Bitmap(AssetLoader.Open(new Uri("avares://TableCloth3/Assets/Images/Spork.png")));

    [ObservableProperty]
    private string _displayName = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _categoryDisplayName = string.Empty;

    [ObservableProperty]
    private string _targetUrl = string.Empty;

    [ObservableProperty]
    private ObservableCollection<TableClothPackageItemViewModel> _packages = new();

    [RelayCommand]
    private void LaunchSite(string serviceId)
        => _messenger.Send<LaunchSiteRequest>(new(this));
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
    private string _categoryDisplayName = string.Empty;

    [ObservableProperty]
    private bool _isWildcard = false;
}
