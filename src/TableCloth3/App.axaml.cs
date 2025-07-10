using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Launcher.Windows;
using TableCloth3.Shared.Contracts;
using TableCloth3.Shared.Services;

namespace TableCloth3;

internal partial class App : Application
{
    [ActivatorUtilitiesConstructor]
    public App(
        AvaloniaWindowManager windowManager,
        IInitializationService initializationService)
        : this()
    {
        _windowManager = windowManager;
        _initializationService = initializationService;
    }

    public App()
        : base()
    {
    }

    private readonly AvaloniaWindowManager _windowManager = default!;
    private readonly IInitializationService _initializationService = default!;

    public override void Initialize()
        => AvaloniaXamlLoader.Load(this);

    public override async void OnFrameworkInitializationCompleted()
    {
        RequestedThemeVariant = ThemeVariant.Default;

        if (!Design.IsDesignMode)
        {
            var splashWindow = _windowManager.GetAvaloniaWindow<LoadingSplashWindow>();
            splashWindow.Show();

            await _initializationService.InitializeAsync(
                Environment.GetCommandLineArgs().Skip(1).ToArray(),
                default);

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
