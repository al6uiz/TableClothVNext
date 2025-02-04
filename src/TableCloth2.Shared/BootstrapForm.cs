using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Shared;
using TableCloth2.Shared.Contracts;
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
        IBootstrapper bootstrapper,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        SuspendLayout();

        Load += _viewModel.InitializeCommand.ToEventHandler();

        statusLabel.Bind(c => c.Text, _viewModel, v => v.StatusMessage);

        messenger.Register<AsyncRequestMessage<bool>, int>(
            this, (int)Messages.MarkBootsrapAsCompleted,
            OnBootstrapResultReceived);

        ResumeLayout();
    }

    private readonly BootstrapViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    private void OnBootstrapResultReceived(object recipient, AsyncRequestMessage<bool> message)
    {
        if (_viewModel.BootstrapSucceed)
        {
            DialogResult = DialogResult.OK;
            message.Reply(true);
            Close();
        }
        else
        {
            DialogResult = DialogResult.Abort;
            message.Reply(false);
        }
    }
}
