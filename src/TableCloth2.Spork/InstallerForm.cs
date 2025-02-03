using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TableCloth2.Shared;
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
        IServiceProvider serviceProvider)
        : this()
    {
        _viewModel = viewModel;
        _viewModel.RenderRequested += _viewModel_RenderRequested;

        _serviceProvider = serviceProvider;

        SuspendLayout();

        Load += _viewModel.InstallCommand.ToEventHandler();

        ResumeLayout();
    }

    private void _viewModel_RenderRequested(object? sender, RelayEventArgs<List<StepViewModel>> e)
    {
        const int stepControlHeight = 50;
        panel.AutoScrollMargin = new Size(panel.AutoScrollMargin.Width, stepControlHeight);

        var list = new List<StepViewModel>(e.Value);
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
    }

    private readonly InstallerViewModel _viewModel = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public InstallerViewModel ViewModel => _viewModel;
}
