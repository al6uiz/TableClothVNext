using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DotNext.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Launcher.ViewModels;

public sealed partial class FolderManageWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public FolderManageWindowViewModel(
        IMessenger messenger,
        AppSettingsManager appSettingsManager)
    {
        _messenger = messenger;
        _appSettingsManager = appSettingsManager;
    }

    public FolderManageWindowViewModel() { }

    private readonly IMessenger _messenger = default!;
    private readonly AppSettingsManager _appSettingsManager = default!;

    public sealed record class DoubleTappedMessage;

    public interface IDoubleTappedMessageRecipient : IRecipient<DoubleTappedMessage>;

    public sealed record class AddFolderButtonMessage;

    public interface IAddFolderButtonMessageRecipient : IRecipient<AddFolderButtonMessage>;

    public sealed record class RemoveFolderButtonMessage;

    public interface IRemoveFolderButtonMessageRecipient : IRecipient<RemoveFolderButtonMessage>;

    public sealed record class ClearAllFoldersButtonMessage;

    public interface IClearAllFoldersButtonMessageRecipient : IRecipient<ClearAllFoldersButtonMessage>;

    public sealed record class CloseButtonMessage;

    public interface ICloseButtonMessageRecipient : IRecipient<CloseButtonMessage>;

    [ObservableProperty]
    private ObservableCollection<string> _folders = new ObservableCollection<string>();

    [RelayCommand]
    private async Task LoadConfiguration(CancellationToken cancellationToken = default)
        => await _appSettingsManager.LoadAsync(this, cancellationToken).ConfigureAwait(false);

    [RelayCommand]
    private async Task SaveConfiguration(CancellationToken cancellationToken = default)
        => await _appSettingsManager.SaveAsync(this, cancellationToken).ConfigureAwait(false);

    [RelayCommand]
    private void DoubleTapped()
    {
        _messenger.Send<DoubleTappedMessage>();
    }

    [RelayCommand]
    private void AddFolderButton()
    {
        _messenger.Send<AddFolderButtonMessage>();
    }

    [RelayCommand]
    private void RemoveFolderButton()
    {
        _messenger.Send<RemoveFolderButtonMessage>();
    }

    [RelayCommand]
    private void ClearAllFoldersButton()
    {
        _messenger.Send<ClearAllFoldersButtonMessage>();
    }

    [RelayCommand]
    private void CloseButton()
    {
        _messenger.Send<CloseButtonMessage>();
    }

    public override void PopulateForSerialization(IDictionary<string, object?> propertyBag)
    {
        propertyBag[nameof(Folders)] = Folders.ToArray();
        base.PopulateForSerialization(propertyBag);
    }

    public override void PopulateForDeserialization(IReadOnlyDictionary<string, object?> propertyBag)
    {
        Folders.Clear();
        var items = (propertyBag[nameof(Folders)] as IEnumerable<string> ?? Enumerable.Empty<string>());

        foreach (var item in items)
            Folders.Add(item);

        base.PopulateForDeserialization(propertyBag);
    }
}
