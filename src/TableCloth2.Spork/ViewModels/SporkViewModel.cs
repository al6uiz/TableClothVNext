using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Xml.Serialization;
using TableCloth2.Contracts;
using TableCloth2.Services;
using TableCloth2.Shared.Models.Catalog;
using WindowsFormsLifetime;

namespace TableCloth2.Spork.ViewModels;

public sealed partial class SporkViewModel : ObservableObject
{
    public SporkViewModel(
        ILogger<SporkViewModel> logger,
        Configurations configurations,
        IHostApplicationLifetime lifetime,
        IFormProvider formProvider,
        IMessageBoxService messageBoxService,
        KnownPathsService knownPathsService,
        IMessenger messenger)
    {
        _logger = logger;
        _configurations = configurations;
        _lifetime = lifetime;
        _formProvider = formProvider;
        _messageBoxService = messageBoxService;
        _knownPathsService = knownPathsService;
        _messenger = messenger;
    }

    private readonly ILogger _logger;
    private readonly Configurations _configurations;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly IFormProvider _formProvider;
    private readonly IMessageBoxService _messageBoxService;
    private readonly KnownPathsService _knownPathsService;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private Dictionary<string, string> _images = new Dictionary<string, string>();

    [ObservableProperty]
    private Dictionary<string, string> _icons = new Dictionary<string, string>();

    [ObservableProperty]
    private HashSet<CatalogInternetService> _selectedServices = new HashSet<CatalogInternetService>();

    [ObservableProperty]
    private CatalogDocument _catalog = default!;

    [RelayCommand]
    private void Launch()
    {
        try
        {
            if (this.SelectedServices.Count < 1)
                return;

            using var installerForm = _formProvider.GetForm<InstallerForm>();

            foreach (var eachService in this.SelectedServices)
                installerForm.ViewModel.Services.Add(eachService);

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

    [RelayCommand]
    private async Task InitializeAsync()
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
                    this.Images[fileName] = image;
                }

                foreach (var icon in Directory.EnumerateFiles(imagesPath, "*.ico"))
                {
                    var fileName = Path.GetFileNameWithoutExtension(icon);
                    this.Icons[fileName] = icon;
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

            this.Catalog = catalog;
        });

        await _messenger.Send<AsyncRequestMessage<bool>, int>((int)Messages.LoadImageList);
    }
}
