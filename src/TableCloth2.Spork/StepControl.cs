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

        stateLabel
            .Bind(c => c.Text, _viewModel, vm => vm.StepSucceed)
            .ApplyValueConverter((_, e) =>
            {
                switch (e.Value)
                {
                    case null:
                        e.Value = "\u25B6";
                        return;
                    case false:
                        e.Value = "\u2716";
                        return;
                    case true:
                        ((ScrollableControl?)Parent)?.ScrollControlIntoView(this);
                        e.Value = "\u2714";
                        return;
                }
            });

        stepNameLabel.Bind(c => c.Text, _viewModel, vm => vm.StepName);
        resultLabel.Bind(c => c.Text, _viewModel, vm => vm.Result);

        ResumeLayout();
    }

    private readonly StepViewModel _viewModel = default!;

    public StepViewModel ViewModel => _viewModel;
}
