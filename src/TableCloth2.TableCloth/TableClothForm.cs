using TableCloth2.TableCloth.ViewModels;

namespace TableCloth2.TableCloth;

public partial class TableClothForm : Form
{
    internal TableClothForm()
    {
        InitializeComponent();

        instructLabel.Font = new Font(instructLabel.Font, FontStyle.Bold);
    }

    public TableClothForm(
        TableClothViewModel viewModel)
        : this()
    {
        _viewModel = viewModel;

        SuspendLayout();

        Load += viewModel.InitializeEvent.ToEventHandler();
        FormClosed += (_, e) => { if (viewModel.CleanupEvent.CanExecute(e)) viewModel.CleanupEvent.Execute(e); };

        settingsButton.Bind(c => c.Command, _viewModel, v => v.ChangeSettingsCommand);
        launchButton.Bind(c => c.Command, _viewModel, v => v.LaunchCommand);

        ResumeLayout();
    }

    private readonly TableClothViewModel _viewModel = default!;

    public TableClothViewModel ViewModel => _viewModel;
}
