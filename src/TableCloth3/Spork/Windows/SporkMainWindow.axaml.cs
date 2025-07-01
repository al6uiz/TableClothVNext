using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Spork.ViewModels;

namespace TableCloth3.Spork.Windows;

public partial class SporkMainWindow : Window
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindow(
        SporkMainWindowViewModel viewModel)
        : this()
    {
        _viewModel = viewModel;

        DataContext = _viewModel;
    }

    public SporkMainWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly SporkMainWindowViewModel _viewModel = default!;
}
