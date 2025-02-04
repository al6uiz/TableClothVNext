using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Shared;
using TableCloth2.Shared.Models.Catalog;
using TableCloth2.Spork.Services;

namespace TableCloth2.Spork.ViewModels;

public sealed partial class InstallerViewModel : ObservableObject
{
    public InstallerViewModel(
        StepFactory stepFactory,
        IMessenger messenger)
    {
        _stepFactory = stepFactory;

        _message = string.Empty;
        _services = new ObservableListSource<CatalogInternetService>();
        _steps = new ObservableListSource<StepViewModel>();
        _messenger = messenger;
    }

    private readonly StepFactory _stepFactory;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private ObservableListSource<CatalogInternetService> _services;

    [ObservableProperty]
    private ObservableListSource<StepViewModel> _steps;

    [RelayCommand]
    private async Task InstallAsync()
    {
        var packages = this.Services.SelectMany(x => x.Packages).DistinctBy(x => x.Url);
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

        foreach (var eachStep in Enumerable.Concat(downloadSteps, installerSteps))
            this.Steps.Add(eachStep);

        if (!await _messenger.Send(new AsyncRequestMessage<bool>(), (int)Messages.RenderSteps))
            throw new Exception("Failed to render installer steps.");

        foreach (var eachStep in this.Steps)
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
}
