using AsyncAwaitBestPractices;
using System.Windows.Input;
using TableCloth2.Shared;
using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Services;

namespace TableCloth2.Spork.ViewModels;

public sealed class InstallerViewModel : ViewModelBase
{
    public InstallerViewModel(
        StepFactory stepFactory)
    {
        _stepFactory = stepFactory;

        _message = string.Empty;
        _services = new List<CatalogInternetService>();
        _steps = new List<StepViewModel>();

        _installCommand = new RelayCommand(Install);
    }

    private readonly StepFactory _stepFactory;

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
        var packages = _services.SelectMany(x => x.Packages).DistinctBy(x => x.Url);
        var downloadSteps = new List<StepViewModel>();
        var installerSteps = new List<StepViewModel>();

        foreach (var eachPackage in packages)
        {
            var downloadStep = _stepFactory.CreateDownloadStep(eachPackage);
            downloadSteps.Add(new StepViewModel()
            {
                StepName = downloadStep.StepName,
                InstallerStep = downloadStep,
            });

            var installerStep = _stepFactory.CreateInstallerStep(eachPackage);
            installerSteps.Add(new StepViewModel()
            {
                StepName = installerStep.StepName,
                InstallerStep = installerStep,
            });
        }

        _steps.AddRange(downloadSteps);
        _steps.AddRange(installerSteps);

        RenderRequested?.Invoke(this, new RelayEventArgs<List<StepViewModel>>(_steps));

        foreach (var eachStep in _steps)
        {
            eachStep.IsActiveStep = true;
            eachStep.Result = "In Progress...";

            try
            {
                await eachStep.InstallerStep.PerformStepAsync();
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
