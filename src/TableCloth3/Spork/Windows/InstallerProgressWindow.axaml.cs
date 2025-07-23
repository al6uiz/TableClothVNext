using System.Diagnostics;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using FluentAvalonia.UI.Windowing;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Spork.Contracts;
using TableCloth3.Spork.ViewModels;
using static TableCloth3.Spork.ViewModels.InstallerProgressWindowViewModel;

namespace TableCloth3;

public partial class InstallerProgressWindow :
    AppWindow,
    ITaskBarProgressHost,
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

        TitleBar.ExtendsContentIntoTitleBar = true;
        TitleBar.TitleBarHitTestType = TitleBarHitTestType.Complex;

        ShowAsDialog = true;

        ViewModel.TaskBarProgressService.ProgressHost = this;

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

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        ViewModel.Release();
    }

    void IRecipient<CancelNotification>.Receive(CancelNotification message)
    {
        if (message.dueToError) // If the cancellation is due to an error, we want to show the reason.
			return;

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

            ViewModel.Release();

            Close();
        });
    }

    public void Reset()
    {
        if (Dispatcher.UIThread.CheckAccess())
            PlatformFeatures.SetTaskBarProgressBarState(TaskBarProgressBarState.None);
        else
            Dispatcher.UIThread.Invoke(() => Reset());
    }

    public void SetProgress(bool hasError, ulong currentValue, ulong totalValue)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            PlatformFeatures.SetTaskBarProgressBarState(hasError ? TaskBarProgressBarState.Error : TaskBarProgressBarState.Normal);
            PlatformFeatures.SetTaskBarProgressBarValue(currentValue, totalValue);
        }
        else
            Dispatcher.UIThread.Invoke(() => SetProgress(hasError, currentValue, totalValue));
    }
}

public class BooleanToOpacityConverter : MarkupExtension, IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is bool b && b ? 1.0 : 0.0;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is double opacity && opacity != 0.0;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}