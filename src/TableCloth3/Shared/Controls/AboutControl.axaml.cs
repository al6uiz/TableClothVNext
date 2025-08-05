using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml;
using System.Windows.Input;

namespace TableCloth3.Shared.Controls;

public partial class AboutControl : UserControl
{
    public static readonly StyledProperty<string?> VersionInfoProperty =
        AvaloniaProperty.Register<AboutControl, string?>(nameof(VersionInfo));
    public static readonly StyledProperty<ICommand?> VisitWebSiteCommandProperty =
        AvaloniaProperty.Register<AboutControl, ICommand?>(nameof(VisitWebSiteCommand));
    public static readonly StyledProperty<ICommand?> VisitGitHubCommandProperty =
        AvaloniaProperty.Register<AboutControl, ICommand?>(nameof(VisitGitHubCommand));
    public static readonly StyledProperty<ICommand?> CheckUpdateCommandProperty =
        AvaloniaProperty.Register<AboutControl, ICommand?>(nameof(CheckUpdateCommand));

    public AboutControl()
    {
        InitializeComponent();
    }

    public string? VersionInfo
    {
        get => GetValue(VersionInfoProperty);
        set => SetValue(VersionInfoProperty, value);
    }

    public ICommand? VisitWebSiteCommand
    {
        get => GetValue(VisitWebSiteCommandProperty);
        set => SetValue(VisitWebSiteCommandProperty, value);
    }

    public ICommand? VisitGitHubCommand
    {
        get => GetValue(VisitGitHubCommandProperty);
        set => SetValue(VisitGitHubCommandProperty, value);
    }

    public ICommand? CheckUpdateCommand
    {
        get => GetValue(CheckUpdateCommandProperty);
        set => SetValue(CheckUpdateCommandProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        this.VersionInfoBlock.Bind(TextBox.TextProperty, this.GetObservable(VersionInfoProperty));
        this.VisitWebSiteButton.Bind(Button.CommandProperty, this.GetObservable(VisitWebSiteCommandProperty));
        this.VisitGitHubButton.Bind(Button.CommandProperty, this.GetObservable(VisitGitHubCommandProperty));
        this.CheckUpdateButton.Bind(Button.CommandProperty, this.GetObservable(CheckUpdateCommandProperty));
    }
}
