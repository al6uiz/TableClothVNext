using AsyncAwaitBestPractices;
using System.Windows.Input;
using TableCloth2.Shared;
using TableCloth2.Shared.Contracts;
using TableCloth2.Shared.Models;

namespace TableCloth2.ViewModels;

public sealed partial class BootstrapViewModel : ViewModelBase
{
    public BootstrapViewModel(
        IBootstrapper bootstrapper)
    {
        _bootstrapper = bootstrapper;

        _initializeEvent = new RelayCommand(Initialize);

        _statusMessage = "In Progress...";
    }

    private readonly IBootstrapper _bootstrapper;

    internal ICommand InitializeEvent => _initializeEvent;

    private readonly RelayCommand _initializeEvent;

    private void Initialize(object? _)
        => InitializeAsync(_).SafeFireAndForget();

    private async Task InitializeAsync(object? _)
    {
        StatusMessage = "Preparing...";

        var result = await _bootstrapper.PerformBootstrapAsync();

        if (!result.IsSuccessful)
            StatusMessage = "Bootstrap Failed.";
        else
            StatusMessage = "Completed.";

        BootstrapCompleted?.Invoke(this, new RelayEventArgs<BootstrapResult>(result));
    }

    private string _statusMessage;

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetField(ref _statusMessage, value);
    }

    public event EventHandler<RelayEventArgs<BootstrapResult>>? BootstrapCompleted;
}
