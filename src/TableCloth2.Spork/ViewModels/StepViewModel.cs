using TableCloth2.Spork.Contracts;
using TableCloth2.Spork.Steps;

namespace TableCloth2.Spork.ViewModels;

public sealed class StepViewModel : ViewModelBase
{
    public StepViewModel()
    {
        _isActiveStep = false;
        _stepName = string.Empty;
        _result = string.Empty;
        _stepSucceed = null;
        _installerStep = NoOpStep.Instance;
    }

    private bool _isActiveStep;
    private string _stepName;
    private string _result;
    private bool? _stepSucceed;
    private IInstallerStep _installerStep;

    public bool IsActiveStep
    {
        get => _isActiveStep;
        set => SetField(ref _isActiveStep, value);
    }

    public string StepName
    {
        get => _stepName;
        set => SetField(ref _stepName, value);
    }

    public string Result
    {
        get => _result;
        set => SetField(ref _result, value);
    }

    public bool? StepSucceed
    {
        get => _stepSucceed;
        set => SetField(ref _stepSucceed, value);
    }

    public IInstallerStep InstallerStep
    {
        get => _installerStep;
        set => SetField(ref _installerStep, value);
    }
}
