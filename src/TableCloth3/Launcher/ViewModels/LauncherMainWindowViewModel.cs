using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace TableCloth3.Launcher.ViewModels;

public sealed partial class LauncherMainWindowViewModel : ObservableObject
{
    [ActivatorUtilitiesConstructor]
    public LauncherMainWindowViewModel(
        IMessenger messenger)
    {
        _messenger = messenger;
    }

    public LauncherMainWindowViewModel() { }

    private readonly IMessenger _messenger = default!;

    public sealed record class AboutButtonMessage;

    public interface IAboutButtonMessageRecipient : IRecipient<AboutButtonMessage>;

    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    public sealed record class ManageFolderButtonMessage;

    public interface IManageFolderButtonMessageRecipient : IRecipient<ManageFolderButtonMessage>;

    [ObservableProperty]
    private bool _useMicrophone = false;

    [ObservableProperty]
    private bool _useWebCamera = false;

    [ObservableProperty]
    private bool _sharePrinters = false;

    [ObservableProperty]
    private bool _mountNpkiFolders = true;

    [ObservableProperty]
    private bool _mountSpecificFolders = false;

    [RelayCommand]
    private void AboutButton()
        => _messenger.Send<AboutButtonMessage>();

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonMessage>();

    [RelayCommand]
    private void ManageFolderButton()
        => _messenger.Send<ManageFolderButtonMessage>();
}
