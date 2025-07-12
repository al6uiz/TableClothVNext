using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TableCloth3.Spork.ViewModels;
using static TableCloth3.Spork.ViewModels.InstallerProgressWindowViewModel;

namespace TableCloth3;

public partial class InstallerProgressWindow :
    Window,
    ICancelNotificationRecipient,
    IFinishNotificationRecipient
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

        _messenger.Register<CancelNotification>(this);
        _messenger.Register<FinishNotification>(this);
    }

    public InstallerProgressWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly InstallerProgressWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    public InstallerProgressWindowViewModel ViewModel => _viewModel;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<CancelNotification>.Receive(CancelNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            Close();
        });
    }

    void IRecipient<FinishNotification>.Receive(FinishNotification message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            var psi = new ProcessStartInfo("https://yourtablecloth.app/");
            psi.UseShellExecute = true;
            Process.Start(psi);
            Close();
        });
    }
}