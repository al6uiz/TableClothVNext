using TableCloth2.Shared;
using TableCloth2.Shared.Contracts;
using TableCloth2.Shared.Models;
using TableCloth2.ViewModels;

namespace TableCloth2;

public partial class BootstrapForm : Form
{
    internal BootstrapForm()
        : base()
    {
        InitializeComponent();
    }

    public BootstrapForm(
        BootstrapViewModel viewModel,
        IBootstrapper bootstrapper)
        : this()
    {
        _viewModel = viewModel;

        SuspendLayout();

        Load += _viewModel.InitializeEvent.ToEventHandler();

        statusLabel.Bind(c => c.Text, _viewModel, v => v.StatusMessage);

        _viewModel.BootstrapCompleted += ViewModel_BootstrapCompleted;

        ResumeLayout();
    }

    private void ViewModel_BootstrapCompleted(object? sender, RelayEventArgs<BootstrapResult> e)
    {
        if (e.Value.IsSuccessful)
        {
            DialogResult = DialogResult.OK;
            Close();
            return;
        }
        else
            DialogResult = DialogResult.Abort;
    }

    private readonly BootstrapViewModel _viewModel = default!;
}
