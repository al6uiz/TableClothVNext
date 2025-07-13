using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Help.ViewModels;

public sealed partial class HelpMainWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public HelpMainWindowViewModel(
        IMessenger messenger)
        : this()
    {
        _messenger = messenger;
    }

    public HelpMainWindowViewModel()
        : base()
    {
    }

    private readonly IMessenger _messenger = default!;

    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    [RelayCommand]
    private void CloseButton()
        => _messenger.Send<CloseButtonMessage>();
}
