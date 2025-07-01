using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using TableCloth3.Shared.Languages;
using TableCloth3.Shared.ViewModels;
using static TableCloth3.Shared.ViewModels.AboutWindowViewModel;

namespace TableCloth3.Shared.Windows;

public partial class AboutWindow :
    Window,
    IVisitWebSiteButtonMessageRecipient,
    IVisitGitHubButtonMessageRecipient,
    ICheckUpdateButtonMessageRecipient,
    ICloseButtonMessageRecipient
{
    [ActivatorUtilitiesConstructor]
    public AboutWindow(
        AboutWindowViewModel viewModel,
        IMessenger messenger)
        : this()
    {
        _viewModel = viewModel;
        _messenger = messenger;

        DataContext = _viewModel;

        _messenger.Register<VisitWebSiteButtonMessage>(this);
        _messenger.Register<VisitGitHubButtonMessage>(this);
        _messenger.Register<CheckUpdateButtonMessage>(this);
        _messenger.Register<CloseButtonMessage>(this);
    }

    public AboutWindow()
        : base()
    {
        InitializeComponent();
    }

    private readonly AboutWindowViewModel _viewModel = default!;
    private readonly IMessenger _messenger = default!;

    protected override void OnClosed(EventArgs e)
    {
        _messenger.UnregisterAll(this);
        base.OnClosed(e);
    }

    void IRecipient<VisitWebSiteButtonMessage>.Receive(VisitWebSiteButtonMessage message)
    {
        if (!Uri.TryCreate(SharedStrings.ProjectWebSiteUrl, UriKind.Absolute, out var parsedUri) ||
            parsedUri == null)
            return;

        var startInfo = new ProcessStartInfo(parsedUri.AbsoluteUri)
        {
            UseShellExecute = true,
        };

        Process.Start(startInfo);
    }

    void IRecipient<VisitGitHubButtonMessage>.Receive(VisitGitHubButtonMessage message)
    {
        if (!Uri.TryCreate(SharedStrings.GitHubUrl, UriKind.Absolute, out var parsedUri) ||
            parsedUri == null)
            return;

        var startInfo = new ProcessStartInfo(parsedUri.AbsoluteUri)
        {
            UseShellExecute = true,
        };

        Process.Start(startInfo);
    }

    void IRecipient<CheckUpdateButtonMessage>.Receive(CheckUpdateButtonMessage message)
    {
        // TODO
    }

    void IRecipient<CloseButtonMessage>.Receive(CloseButtonMessage message)
        => Close();
}
