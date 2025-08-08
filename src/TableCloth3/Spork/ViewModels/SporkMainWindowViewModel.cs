using Avalonia.Controls;
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
        CatalogItems.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasCatalogItems));
            OnPropertyChanged(nameof(HasNoCatalogItems));
            OnPropertyChanged(nameof(IsCatalogLoading));
            OnPropertyChanged(nameof(FilteredCatalogItems));
        };

        AddonItems.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(HasAddonItems));
            OnPropertyChanged(nameof(HasNoAddonItems));
            OnPropertyChanged(nameof(IsAddonLoading));
            OnPropertyChanged(nameof(FilteredAddonItems));
        };

        ApplyCatalogFilter();
    }

    // Catalog

    partial void OnIsCatalogLoadingChanged(bool value)
        => OnPropertyChanged(nameof(IsCatalogLoadingCompleted));

    public sealed record class LoadingFailureNotification(Exception OccurredException);

    public interface ILoadingFailureNotificationRecipient : IRecipient<LoadingFailureNotification>;
    
    public sealed record class CloseButtonRequest;

    public interface ICloseButtonRequestRecipient : IRecipient<CloseButtonRequest>;

    private readonly IMessenger _messenger = default!;
    private readonly TableClothCatalogService _catalogService = default!;
    private readonly AvaloniaViewModelManager _avaloniaViewModelManager = default!;
    private readonly LocationService _sporkLocationService = default!;
    private readonly ScenarioRouter _scenarioRouter = default!;

    protected override void PrepareDesignTimePreview()
    {
        CatalogCategoryItems.Add(new() { CategoryName = "Financing", IsWildcard = false, });

        for (var i = 0; i < 100; i++)
        {
            CatalogItems.Add(new(default!, default!, default!)
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
    private ObservableCollection<TableClothCatalogItemViewModel> _catalogItems = [];

    [ObservableProperty]
    private IEnumerable<TableClothCatalogItemViewModel> _filteredCatalogItems = [];

    [ObservableProperty]
    private string _catalogFilterText = string.Empty;

    [ObservableProperty]
    private TableClothCategoryItemViewModel? _selectedCatalogCategory = default;

    [ObservableProperty]
    private ObservableCollection<TableClothCategoryItemViewModel> _catalogCategoryItems = [];

    [ObservableProperty]
    private bool _isCatalogLoading = false;

    public bool IsCatalogLoadingCompleted => !IsCatalogLoading;

    public bool HasCatalogItems
    {
        get
        {
            try { return FilteredCatalogItems.Any(); }
            catch { return false; }
        }
    }

    public bool HasNoCatalogItems => !HasCatalogItems;

    partial void OnCatalogFilterTextChanged(string value)
        => ApplyCatalogFilter();

    partial void OnSelectedCatalogCategoryChanged(TableClothCategoryItemViewModel? value)
        => ApplyCatalogFilter();

    [RelayCommand]
    private void ApplyCatalogFilter()
    {
        var catalogQuery = (IEnumerable<TableClothCatalogItemViewModel>)CatalogItems;
        if (!string.IsNullOrWhiteSpace(CatalogFilterText))
            catalogQuery = catalogQuery.Where(x => x.DisplayName.Contains(CatalogFilterText, StringComparison.OrdinalIgnoreCase));
        if (SelectedCatalogCategory != null && !SelectedCatalogCategory.IsWildcard)
            catalogQuery = catalogQuery.Where(x => x.Category.Equals(SelectedCatalogCategory.CategoryName, StringComparison.OrdinalIgnoreCase));
        FilteredCatalogItems = catalogQuery;

        var addonQuery = (IEnumerable<TableClothAddonItemViewModel>)AddonItems;
        if (!string.IsNullOrWhiteSpace(CatalogFilterText))
            addonQuery = addonQuery.Where(x => x.DisplayName.Contains(CatalogFilterText, StringComparison.OrdinalIgnoreCase));
        if (SelectedCatalogCategory != null && !SelectedCatalogCategory.IsWildcard)
            addonQuery = addonQuery.Where(x => x.DisplayName.Equals(SelectedCatalogCategory.CategoryName, StringComparison.OrdinalIgnoreCase));
        FilteredAddonItems = addonQuery;
    }

    [RelayCommand]
    private async Task RefreshCatalog(CancellationToken cancellationToken = default)
    {
        try
        {
            IsCatalogLoading = true;
            CatalogItems.Clear();

            if (_scenarioRouter.GetSporkScenario() == SporkScenario.Standalone)
            {
                var catalogDownloadTask = _catalogService.DownloadCatalogAsync(cancellationToken);
                var imageDownloadTask = _catalogService.DownloadImagesAsync(cancellationToken);
                await Task.WhenAll(catalogDownloadTask, imageDownloadTask).ConfigureAwait(false);
            }
            else
            {
                var launcherDataDirectory = _sporkLocationService.GetDesktopSubDirectory("Launcher");
                if (Directory.Exists(launcherDataDirectory))
                {
                    var launcherImagesDirectory = Path.Combine(launcherDataDirectory, "Images");

                    var destAppDataDirectory = _sporkLocationService.EnsureAppDataDirectoryCreated().FullName;
                    var destImagesDirectory = _sporkLocationService.EnsureImagesDirectoryCreated().FullName;

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

                var npkiDirectory = _sporkLocationService.GetDesktopSubDirectory("NPKI");
                if (Directory.Exists(npkiDirectory))
                {
                    var destNPKIDirectory = _sporkLocationService.EnsureLocalLowNpkiDirectoryCreated().FullName;
                    CopyDirectory(npkiDirectory, destNPKIDirectory, true);
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

                CatalogItems.Add(viewModel);
            }
        }
        catch (Exception ex)
        {
            _messenger?.Send<LoadingFailureNotification>(new(ex));
        }
        finally
        {
            IsCatalogLoading = false;
        }

        CatalogCategoryItems.Clear();

        var allItemCategory = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothCategoryItemViewModel>();
        allItemCategory.CategoryName = "All";
        allItemCategory.CategoryDisplayName = SharedStrings.AllCategoryDisplayName;
        allItemCategory.IsWildcard = true;

        CatalogCategoryItems.Add(allItemCategory);
        SelectedCatalogCategory = allItemCategory;
        foreach (var eachCategory in CatalogItems.Select(x => x.Category).Distinct())
        {
            var eachItem = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothCategoryItemViewModel>();
            eachItem.CategoryName = eachCategory;
            eachItem.CategoryDisplayName = _catalogService.GetCatalogDisplayName(eachCategory);
            eachItem.IsWildcard = false;
            CatalogCategoryItems.Add(eachItem);
        }

        ApplyCatalogFilter();
    }

    // Addons

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken = default)
    {
        if (Design.IsDesignMode)
            return;

        await RefreshCatalog(cancellationToken).ConfigureAwait(false);
        await RefreshAddon(cancellationToken).ConfigureAwait(false);
    }

    [ObservableProperty]
    private ObservableCollection<TableClothAddonItemViewModel> _addonItems = [];

    [ObservableProperty]
    private IEnumerable<TableClothAddonItemViewModel> _filteredAddonItems = [];

    [ObservableProperty]
    private string _addonFilterText = string.Empty;

    [ObservableProperty]
    private bool _isAddonLoading = false;

    public bool IsAddonLoadingCompleted => !IsAddonLoading;

    public bool HasAddonItems
    {
        get
        {
            try { return FilteredAddonItems.Any(); }
            catch { return false; }
        }
    }

    public bool HasNoAddonItems => !HasAddonItems;

    partial void OnAddonFilterTextChanged(string value)
        => ApplyAddonFilter();

    [RelayCommand]
    private void ApplyAddonFilter()
    {
        var query = (IEnumerable<TableClothAddonItemViewModel>)AddonItems;
        if (!string.IsNullOrWhiteSpace(AddonFilterText))
            query = query.Where(x => x.DisplayName.Contains(AddonFilterText, StringComparison.OrdinalIgnoreCase));
        FilteredAddonItems = query;
    }

    [RelayCommand]
    private async Task RefreshAddon(CancellationToken cancellationToken = default)
    {
        try
        {
            IsAddonLoading = true;
            AddonItems.Clear();

            if (_scenarioRouter.GetSporkScenario() == SporkScenario.Standalone)
            {
                var catalogDownloadTask = _catalogService.DownloadCatalogAsync(cancellationToken);
                var imageDownloadTask = _catalogService.DownloadImagesAsync(cancellationToken);
                await Task.WhenAll(catalogDownloadTask, imageDownloadTask).ConfigureAwait(false);
            }
            else
            {
                var launcherDataDirectory = _sporkLocationService.GetDesktopSubDirectory("Launcher");
                if (Directory.Exists(launcherDataDirectory))
                {
                    var launcherImagesDirectory = Path.Combine(launcherDataDirectory, "Images");

                    var destAppDataDirectory = _sporkLocationService.EnsureAppDataDirectoryCreated().FullName;
                    var destImagesDirectory = _sporkLocationService.EnsureImagesDirectoryCreated().FullName;

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

                var npkiDirectory = _sporkLocationService.GetDesktopSubDirectory("NPKI");
                if (Directory.Exists(npkiDirectory))
                {
                    var destNPKIDirectory = _sporkLocationService.EnsureLocalLowNpkiDirectoryCreated().FullName;
                    CopyDirectory(npkiDirectory, destNPKIDirectory, true);
                }
            }

            var doc = await _catalogService.LoadCatalogAsync(cancellationToken).ConfigureAwait(false);
            var addons = doc.XPathSelectElements("/TableClothCatalog/Companions/Companion");

            foreach (var eachAddon in addons)
            {
                var id = eachAddon.Attribute("Id")?.Value;

                if (string.IsNullOrWhiteSpace(id))
                    continue;

                var displayName = eachAddon.Attribute("DisplayName")?.Value ?? id;
                var url = eachAddon.Attribute("Url")?.Value;
                var arguments = eachAddon.Attribute("Arguments")?.Value ?? string.Empty;

                var viewModel = _avaloniaViewModelManager.GetAvaloniaViewModel<TableClothAddonItemViewModel>();
                viewModel.AddonId = id;
                viewModel.DisplayName = displayName;
                viewModel.Arguments = arguments;
                viewModel.TargetUrl = url ?? string.Empty;

                AddonItems.Add(viewModel);
            }
        }
        catch (Exception ex)
        {
            _messenger?.Send<LoadingFailureNotification>(new(ex));
        }
        finally
        {
            IsAddonLoading = false;
        }

        ApplyAddonFilter();
    }

    // Shared

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonRequest>();

    private void CopyDirectory(string sourceDir, string destinationDir, bool recursive)
    {
        var dir = new DirectoryInfo(sourceDir);

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir}");

        var subDirs = dir.GetDirectories();

        Directory.CreateDirectory(destinationDir);

        foreach (FileInfo file in dir.GetFiles())
        {
            var targetFilePath = Path.Combine(destinationDir, file.Name);
            file.CopyTo(targetFilePath, overwrite: true);
        }

        if (recursive)
        {
            foreach (DirectoryInfo subDir in subDirs)
            {
                var newDestinationDir = Path.Combine(destinationDir, subDir.Name);
                CopyDirectory(subDir.FullName, newDestinationDir, recursive);
            }
        }
    }
}
