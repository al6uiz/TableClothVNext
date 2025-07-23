using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Help.ViewModels;
using static TableCloth3.Help.ViewModels.HelpMainWindowViewModel;

namespace TableCloth3;

public partial class HelpMainWindow : AppWindow, ICloseButtonMessageRecipient
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

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        ShowAsDialog = true;

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
