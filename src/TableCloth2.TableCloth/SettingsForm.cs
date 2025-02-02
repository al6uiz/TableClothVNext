using TableCloth2.Shared;
using TableCloth2.TableCloth.ViewModels;

namespace TableCloth2.TableCloth;

public partial class SettingsForm : Form
{
    internal SettingsForm()
    {
        InitializeComponent();
    }

    public SettingsForm(
        SettingsViewModel viewModel)
        : this()
    {
        _viewModel = viewModel;

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

        _viewModel.FolderSelect += viewModel_FolderSelect;

        ResumeLayout();
    }

    private void viewModel_FolderSelect(object? sender, RelayEventArgs<List<string>> e)
    {
        if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
        {
            e.Value.AddRange(folderBrowserDialog.SelectedPaths);
            e.Accepted = true;
        }
    }

    private readonly SettingsViewModel _viewModel = default!;

    public SettingsViewModel ViewModel => _viewModel;
}
