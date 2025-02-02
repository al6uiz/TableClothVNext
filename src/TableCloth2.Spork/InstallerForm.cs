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

    private void _viewModel_RenderRequested(object? sender, EventArgs e)
    {
        foreach (var eachService in _viewModel.Services)
        {
            foreach (var eachStep in eachService.Packages)
            {
                var stepControl = _serviceProvider.GetRequiredService<StepControl>();
                stepControl.ViewModel.StepName = eachStep.Name;
                stepControl.ViewModel.IsActiveStep = false;
                stepControl.ViewModel.StepProgress = 0;
                stepControl.ViewModel.StepSucceed = null;

                stepControl.Parent = this.panel;
                stepControl.Dock = DockStyle.Top;
            }
        }
    }

    private readonly InstallerViewModel _viewModel = default!;
    private readonly IServiceProvider _serviceProvider = default!;

    public InstallerViewModel ViewModel => _viewModel;
}
