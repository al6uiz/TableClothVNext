using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Launcher.Windows;
using TableCloth3.Shared.Services;

namespace TableCloth3;

internal partial class App : Application
{
    [ActivatorUtilitiesConstructor]
    public App(AvaloniaWindowManager windowManager)
        : this()
    {
        _windowManager = windowManager;
    }

    public App()
        : base()
    {
    }

    private readonly AvaloniaWindowManager _windowManager = default!;

    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        RequestedThemeVariant = ThemeVariant.Default;

        if (!Design.IsDesignMode)
        {
            var splashWindow = _windowManager.GetAvaloniaWindow<LoadingSplashWindow>();
            splashWindow.Show();

            await Task.Delay(TimeSpan.FromSeconds(1d));
            splashWindow.Hide();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = _windowManager.GetMainAvaloniaWindow();
                desktop.MainWindow.Show();
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}
