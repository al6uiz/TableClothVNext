using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Shared;
using TableCloth2.Shared.Contracts;

namespace TableCloth2.ViewModels;

public sealed partial class BootstrapViewModel : ObservableObject
{
    public BootstrapViewModel(
        IBootstrapper bootstrapper,
        IMessenger messenger)
    {
        _bootstrapper = bootstrapper;
        _messenger = messenger;

        statusMessage = "In Progress...";
    }

    private readonly IBootstrapper _bootstrapper;
    private readonly IMessenger _messenger;

    [ObservableProperty]
    private string statusMessage;

    [ObservableProperty]
    private bool bootstrapSucceed;

    [RelayCommand]
    private async Task InitializeAsync()
    {
        StatusMessage = "Preparing...";

        var result = await _bootstrapper.PerformBootstrapAsync();
        BootstrapSucceed = result.IsSuccessful;

        if (BootstrapSucceed)
            StatusMessage = "Completed.";
        else
            StatusMessage = "Bootstrap Failed.";

        await _messenger.Send<AsyncRequestMessage<bool>, int>(
            (int)Messages.MarkBootsrapAsCompleted);
    }
}
