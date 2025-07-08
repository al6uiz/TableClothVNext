using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TableCloth3.Shared.ViewModels;

public sealed partial class AboutWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public AboutWindowViewModel(
        IMessenger messenger)
        : this()
    {
        this.messenger = messenger;
    }

    public AboutWindowViewModel()
        : base()
    {
    }

    private readonly IMessenger messenger = default!;

    public sealed record class VisitWebSiteButtonMessage;

    public interface IVisitWebSiteButtonMessageRecipient : IRecipient<VisitWebSiteButtonMessage>;

    public sealed record class VisitGitHubButtonMessage;

    public interface IVisitGitHubButtonMessageRecipient : IRecipient<VisitGitHubButtonMessage>;

    public sealed record class CheckUpdateButtonMessage;

    public interface ICheckUpdateButtonMessageRecipient : IRecipient<CheckUpdateButtonMessage>;
    
    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    [ObservableProperty]
    private string versionInfo = Assembly
        .GetExecutingAssembly()
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
        ?.InformationalVersion ?? "(untagged build)";

    [RelayCommand]
    private void VisitWebSiteButton()
        => messenger.Send<VisitWebSiteButtonMessage>();

    [RelayCommand]
    private void VisitGitHubButton()
        => messenger.Send<VisitGitHubButtonMessage>();

    [RelayCommand]
    private void CheckUpdateButton()
        => messenger.Send<CheckUpdateButtonMessage>();

    [RelayCommand]
    private void CloseButton()
        => messenger.Send<CloseButtonMessage>();
}
