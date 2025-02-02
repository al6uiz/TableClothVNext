using TableCloth2.Spork.ViewModels;

namespace TableCloth2.Spork;

public partial class StepControl : UserControl
{
    internal StepControl()
        : base()
    {
        InitializeComponent();
    }

    public StepControl(
        StepViewModel viewModel)
        : this()
    {
        _viewModel = viewModel;

        SuspendLayout();

        progressBar.Bind(c => c.Visible, _viewModel, vm => vm.IsActiveStep);
        progressBar.Bind(c => c.Value, _viewModel, vm => vm.StepProgress);

        stateLabel
            .Bind(c => c.Text, _viewModel, vm => vm.StepSucceed)
            .ApplyValueConverter((_, e) =>
            {
                e.Value = e.Value switch
                {
                    true => "\u2714",
                    false => "\u2716",
                    _ => string.Empty
                };
            });

        stepNameLabel.Bind(c => c.Text, _viewModel, vm => vm.StepName);

        ResumeLayout();
    }

    private readonly StepViewModel _viewModel = default!;

    public StepViewModel ViewModel => _viewModel;
}
