using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Help.ViewModels;
using static TableCloth3.Help.ViewModels.HelpMainWindowViewModel;

namespace TableCloth3;

public partial class HelpMainWindow : Window, ICloseButtonMessageRecipient
{
    [ActivatorUtilitiesConstructor]
    public HelpMainWindow(
        HelpMainWindowViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        DataContext = _viewModel;

        _messenger.Register<CloseButtonMessage>(this);
    }

    public HelpMainWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly HelpMainWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<CloseButtonMessage>.Receive(CloseButtonMessage message)
    {
        Close();
    }
}
