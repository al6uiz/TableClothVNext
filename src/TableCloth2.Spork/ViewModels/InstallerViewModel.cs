using AsyncAwaitBestPractices;
using System.Windows.Input;
using TableCloth2.Shared.Models.Catalog;

namespace TableCloth2.Spork.ViewModels;

public sealed class InstallerViewModel : ViewModelBase
{
    public InstallerViewModel()
    {
        _message = string.Empty;
        _services = new List<CatalogInternetService>();
        _installCommand = new RelayCommand(Install);
    }

    private string _message;

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    private List<CatalogInternetService> _services;

    public List<CatalogInternetService> Services
    {
        get => _services;
    }

    private RelayCommand _installCommand;

    public ICommand InstallCommand => _installCommand;

    private void Install(object? _)
        => InstallAsync(_).SafeFireAndForget();

    private async Task InstallAsync(object? _)
    {
        RenderRequested?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? RenderRequested;
}
