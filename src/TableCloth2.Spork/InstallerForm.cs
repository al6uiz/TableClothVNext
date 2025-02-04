using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using TableCloth2.Spork.ViewModels;

namespace TableCloth2.Spork;

public partial class InstallerForm : Form
{
    internal InstallerForm()
        : base()
    {
        InitializeComponent();
    }

    public InstallerForm(
        InstallerViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        _messenger.Register<AsyncRequestMessage<bool>, int>(this, (int)Messages.RenderSteps, OnRenderSteps);

        SuspendLayout();

        Load += _viewModel.InstallCommand.ToEventHandler();

        ResumeLayout();
    }

    private readonly InstallerViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    private void OnRenderSteps(object recipient, AsyncRequestMessage<bool> message)
    {
        const int stepControlHeight = 50;
        panel.AutoScrollMargin = new Size(panel.AutoScrollMargin.Width, stepControlHeight);

        var list = new List<StepViewModel>(_viewModel.Steps);
        list.Reverse();

        foreach (var eachStep in list)
        {
            var stepControl = new StepControl(eachStep)
            {
                Parent = panel,
                Dock = DockStyle.Top,
                Height = stepControlHeight,
            };
        }

        message.Reply(true);
    }

    public InstallerViewModel ViewModel => _viewModel;
}
