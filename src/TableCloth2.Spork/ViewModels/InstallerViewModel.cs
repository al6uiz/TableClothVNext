using AsyncAwaitBestPractices;
using System.Windows.Input;
using TableCloth2.Shared;
using TableCloth2.Shared.Models.Catalog;
using Windows.Networking.Sockets;

namespace TableCloth2.Spork.ViewModels;

public sealed class InstallerViewModel : ViewModelBase
{
    public InstallerViewModel()
    {
        _message = string.Empty;
        _services = new List<CatalogInternetService>();
        _steps = new List<StepViewModel>();

        _installCommand = new RelayCommand(Install);
    }

    private string _message;

    public string Message
    {
        get => _message;
        set => SetField(ref _message, value);
    }

    private List<CatalogInternetService> _services;
    private List<StepViewModel> _steps;

    public List<CatalogInternetService> Services
    {
        get => _services;
    }

    public List<StepViewModel> Steps
    {
        get => _steps;
    }

    private RelayCommand _installCommand;

    public ICommand InstallCommand => _installCommand;

    private void Install(object? _)
        => InstallAsync(_).SafeFireAndForget();

    private async Task InstallAsync(object? _)
    {
        foreach (var eachService in _services)
        {
            foreach (var eachPackage in eachService.Packages)
            {
                var step = new StepViewModel();
                step.StepName = $"[{eachService.DisplayName}] Downloading {eachPackage}";
                _steps.Add(step);
            }
        }

        foreach (var eachService in _services)
        {
            foreach (var eachPackage in eachService.Packages)
            {
                var step = new StepViewModel();
                step.StepName = $"[{eachService.DisplayName}] Installing {eachPackage}";
                _steps.Add(step);
            }
        }

        RenderRequested?.Invoke(this, new RelayEventArgs<List<StepViewModel>>(_steps));

        foreach (var eachStep in _steps)
        {
            eachStep.IsActiveStep = true;
            eachStep.Result = "In Progress...";

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1d));
                eachStep.StepSucceed = true;
            }
            catch
            {
                eachStep.StepSucceed = false;
            }
            finally
            {
                eachStep.Result = eachStep.StepSucceed.HasValue ?
                    eachStep.StepSucceed.Value ? "Succeed" : "Failed" :
                    "Unknown";
                eachStep.IsActiveStep = false;
            }
        }
    }

    public event EventHandler<RelayEventArgs<List<StepViewModel>>>? RenderRequested;
}
