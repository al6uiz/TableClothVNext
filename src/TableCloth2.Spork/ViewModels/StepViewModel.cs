using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TableCloth2.Spork.ViewModels;

public sealed class StepViewModel : ViewModelBase
{
    private bool _isActiveStep;
    private string _stepName = string.Empty;
    private int _stepProgress = 0;
    private bool? _stepSucceed = null;

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

    public int StepProgress
    {
        get => _stepProgress;
        set => SetField(ref _stepProgress, Math.Min(100, Math.Max(0, value)));
    }

    public bool? StepSucceed
    {
        get => _stepSucceed;
        set => SetField(ref _stepSucceed, value);
    }
}
