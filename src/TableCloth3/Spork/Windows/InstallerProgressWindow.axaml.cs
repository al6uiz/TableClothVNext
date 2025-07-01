using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Spork.ViewModels;

namespace TableCloth3;

public partial class InstallerProgressWindow : Window
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindow(
        InstallerProgressWindowViewModel viewModel)
        : this()
    {
        _viewModel = viewModel;

        DataContext = _viewModel;
    }

    public InstallerProgressWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly InstallerProgressWindowViewModel _viewModel = default!;
}