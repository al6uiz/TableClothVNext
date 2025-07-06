using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Spork.ViewModels;

namespace TableCloth3;

public partial class InstallerProgressWindow : Window
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindow(
        InstallerProgressWindowViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        DataContext = _viewModel;
    }

    public InstallerProgressWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly InstallerProgressWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
}