using AsyncAwaitBestPractices;
using System.Windows.Input;

namespace TableCloth2.Spork.ViewModels;

public sealed class StepViewModel : ViewModelBase
{
    public StepViewModel()
    {
        _initializeEvent = new RelayCommand(Initialize);

        _isActiveStep = false;
        _stepName = string.Empty;
        _result = string.Empty;
        _stepSucceed = null;
    }

    internal ICommand InitializeEvent => _initializeEvent;

    private readonly RelayCommand _initializeEvent;

    private void Initialize(object? _)
        => InitializeAsync(_).SafeFireAndForget();

    private Task InitializeAsync(object? _)
    {
        return Task.CompletedTask;
    }

    private bool _isActiveStep;
    private string _stepName;
    private string _result;
    private bool? _stepSucceed;

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
}
