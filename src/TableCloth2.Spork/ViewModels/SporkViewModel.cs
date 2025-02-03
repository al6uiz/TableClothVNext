using AsyncAwaitBestPractices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Windows.Input;
using System.Xml.Serialization;
using TableCloth2.Contracts;
using TableCloth2.Services;
using TableCloth2.Shared.Models.Catalog;
using WindowsFormsLifetime;

namespace TableCloth2.Spork.ViewModels;

public sealed class SporkViewModel : ViewModelBase
{
    public SporkViewModel(
        ILogger<SporkViewModel> logger,
        Configurations configurations,
        IHostApplicationLifetime lifetime,
        IFormProvider formProvider,
        IMessageBoxService messageBoxService,
        KnownPathsService knownPathsService)
    {
        _logger = logger;
        _configurations = configurations;
        _lifetime = lifetime;
        _formProvider = formProvider;
        _messageBoxService = messageBoxService;
        _knownPathsService = knownPathsService;

        _launchCommand = new RelayCommand(Launch);
        _initializeEvent = new RelayCommand(Initialize);
    }

    private readonly ILogger _logger;
    private readonly Configurations _configurations;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IFormProvider _formProvider;
    private readonly IMessageBoxService _messageBoxService;
    private readonly KnownPathsService _knownPathsService;

    private readonly RelayCommand _launchCommand;

    public ICommand LaunchCommand => _launchCommand;

    private void Launch(object? selectedServicesArgs)
    {
        try
        {
            if (_selectedServices.Count < 1)
                return;

            using var installerForm = _formProvider.GetForm<InstallerForm>();
            installerForm.ViewModel.Services.AddRange(_selectedServices);

            if (installerForm.ShowDialog() != DialogResult.OK)
            {
                _messageBoxService.ShowError("Cannot launch the site.", "Error");
                return;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());
        }
    }

    private readonly RelayCommand _initializeEvent;

    internal ICommand InitializeEvent => _initializeEvent;

    private void Initialize(object? _)
        => InitializeAsync(_).SafeFireAndForget();

    private async Task InitializeAsync(object? _)
    {
        var bootstrapForm = await _formProvider.GetFormAsync<BootstrapForm>();
        if (bootstrapForm.ShowDialog() != DialogResult.OK)
        {
            MessageBox.Show("Bootstrap failed.");
            _lifetime.StopApplication();
            return;
        }

        // 별도 스레드에서 이미지 파일과 카탈로그 문서를 불러들입니다.
        await Task.Run(() =>
        {
            var imagesPath = _knownPathsService.EnsureTableClothSettingsDirectoryExists().Combine("Images");

            if (Directory.Exists(imagesPath))
            {
                foreach (var image in Directory.EnumerateFiles(imagesPath, "*.png"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(image);
                    _images[fileName] = image;
                }

                foreach (var icon in Directory.EnumerateFiles(imagesPath, "*.ico"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(icon);
                    _icons[fileName] = icon;
                }
            }

            var catalogsPath = _knownPathsService.EnsureTableClothSettingsDirectoryExists().Combine("Catalog.xml");

            if (!File.Exists(catalogsPath))
            {
                MessageBox.Show("Catalog.xml not found.");
                _lifetime.StopApplication();
                return;
            }

            using var catalogStream = File.OpenRead(catalogsPath);
            var serializer = new XmlSerializer(typeof(CatalogDocument));
            var catalog = serializer.Deserialize(catalogStream) as CatalogDocument;

            if (catalog == null)
            {
                MessageBox.Show("Catalog.xml is invalid.");
                _lifetime.StopApplication();
                return;
            }

            _catalog = catalog;
        });

        LoadImageListRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? LoadImageListRequested;

    private readonly Dictionary<string, string> _images = new Dictionary<string, string>();
    private readonly Dictionary<string, string> _icons = new Dictionary<string, string>();
    private CatalogDocument _catalog = default!;
    private HashSet<CatalogInternetService> _selectedServices = new HashSet<CatalogInternetService>();

    public IReadOnlyDictionary<string, string> Images => _images;
    public IReadOnlyDictionary<string, string> Icons => _icons;
    public IReadOnlyList<CatalogInternetService> Services => _catalog.Services;
    public HashSet<CatalogInternetService> SelectedServices => _selectedServices;
}
