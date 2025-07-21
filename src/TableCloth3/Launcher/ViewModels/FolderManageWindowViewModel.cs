using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TableCloth3.Launcher.Models;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Launcher.ViewModels;

public sealed partial class FolderManageWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public FolderManageWindowViewModel(
        IMessenger messenger)
    {
        _messenger = messenger;
    }

    public FolderManageWindowViewModel() { }

    private readonly IMessenger _messenger = default!;

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

    public override void ImportFromModel(object? model)
    {
        if (model is FolderSettingsModel e)
        {
            Folders.Clear();
            foreach (var eachFolder in e.Folders)
                Folders.Add(eachFolder);
        }
        base.ImportFromModel(model);
    }

    public override void ExportToModel(object? model)
    {
        if (model is FolderSettingsModel e)
        {
            e.Folders = Folders.ToArray();
        }
        base.ExportToModel(model);
    }
}
