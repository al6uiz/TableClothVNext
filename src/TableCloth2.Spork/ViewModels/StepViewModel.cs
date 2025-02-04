using CommunityToolkit.Mvvm.ComponentModel;
using TableCloth2.Spork.Contracts;
using TableCloth2.Spork.Steps;

namespace TableCloth2.Spork.ViewModels;

public sealed partial class StepViewModel : ObservableObject
{
    public StepViewModel()
    {
        _isActiveStep = false;
        _stepName = string.Empty;
        _result = string.Empty;
        _stepSucceed = null;
        _installerStep = NoOpStep.Instance;
    }

    [ObservableProperty]
    private bool _isActiveStep;

    [ObservableProperty]
    private string _stepName;

    [ObservableProperty]
    private string _result;

    [ObservableProperty]
    private bool? _stepSucceed;

    [ObservableProperty]
    private IInstallerStep _installerStep;
}
