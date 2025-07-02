using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.Windows;
using TableCloth3.Spork.ViewModels;
using static TableCloth3.Spork.ViewModels.SporkMainWindowViewModel;

namespace TableCloth3.Spork.Windows;

public partial class SporkMainWindow :
    Window,
    IRecipient<AboutButtonRequest>,
    IRecipient<CloseButtonRequest>
{
    [ActivatorUtilitiesConstructor]
    public SporkMainWindow(
        SporkMainWindowViewModel viewModel,
        IMessenger messenger,
        AvaloniaWindowManager windowManager)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;
        _windowManager = windowManager;

        _messenger.Register<AboutButtonRequest>(this);
        _messenger.Register<CloseButtonRequest>(this);

        DataContext = _viewModel;
    }

    public SporkMainWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly SporkMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;
    private readonly AvaloniaWindowManager _windowManager = default!;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<AboutButtonRequest>.Receive(SporkMainWindowViewModel.AboutButtonRequest message)
    {
        var aboutWindow = _windowManager.GetAvaloniaWindow<AboutWindow>();
        aboutWindow.ShowDialog(this);
    }

    void IRecipient<CloseButtonRequest>.Receive(SporkMainWindowViewModel.CloseButtonRequest message)
    {
        Close();
    }
}
