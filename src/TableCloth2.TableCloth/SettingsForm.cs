using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.TableCloth.ViewModels;

namespace TableCloth2.TableCloth;

public partial class SettingsForm : Form
{
    internal SettingsForm()
    {
        InitializeComponent();
    }

    public SettingsForm(
        SettingsViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        SuspendLayout();

        mountNPKICerts.Bind(c => c.Checked, _viewModel, v => v.MountNPKICerts);

        enableFolderMount.Bind(c => c.Checked, _viewModel, v => v.EnableFolderMount);

        folderMountList.Bind(_viewModel, v => v.FolderMountList);
        folderMountList.Bind(c => c.Enabled, _viewModel, v => v.EnableFolderMount);
        folderMountList.Bind(c => c.SelectedItem, _viewModel, v => v.SelectedFolderMountPath);

        includeFolderButton.Bind(c => c.Command, _viewModel, v => v.IncludeFolderCommand);
        includeFolderButton.Bind(c => c.Enabled, _viewModel, v => v.EnableFolderMount);

        excludeFolderButton.Bind(c => c.Command, _viewModel, v => v.ExcludeFolderCommand);
        excludeFolderButton.Bind(c => c.Enabled, _viewModel, v => v.EnableFolderMount);

        excludeAllFolderButton.Bind(c => c.Command, _viewModel, v => v.ExcludeAllFoldersCommand);
        excludeAllFolderButton.Bind(c => c.Enabled, _viewModel, v => v.EnableFolderMount);

        enableAudioInput.Bind(c => c.Checked, _viewModel, v => v.EnableAudioInput);
        enableVideoInput.Bind(c => c.Checked, _viewModel, v => v.EnableVideoInput);
        enablePrinterRedirection.Bind(c => c.Checked, _viewModel, v => v.EnablePrinterRedirection);
        enableVirtualizedGpu.Bind(c => c.Checked, _viewModel, v => v.EnableVirtualizedGpu);
        useCloudflareDns.Bind(c => c.Checked, _viewModel, v => v.UseCloudflareDns);

        collectSentryLog.Bind(c => c.Checked, _viewModel, v => v.CollectSentryLog);
        collectAnalytics.Bind(c => c.Checked, _viewModel, v => v.CollectAnalytics);

        _messenger.Register<AsyncRequestMessage<IEnumerable<string>>, int>(
            this, (int)Messages.FolderSelect, OnFolderSelect);

        ResumeLayout();
    }

    private readonly SettingsViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    public SettingsViewModel ViewModel => _viewModel;

    private void OnFolderSelect(object recipient, AsyncRequestMessage<IEnumerable<string>> message)
    {
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            message.Reply(folderBrowserDialog.SelectedPaths);
        else
            message.Reply(Enumerable.Empty<string>());
    }
}
