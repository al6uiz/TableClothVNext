using Avalonia;
using Lemon.Hosting.AvaloniauiDesktop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Runtime.Versioning;
using TableCloth3.Launcher;
using TableCloth3.Shared;
using TableCloth3.Shared.Models;
using TableCloth3.Shared.Services;
using TableCloth3.Spork;

namespace TableCloth3;

public static class Program
{
    [STAThread]
    [SupportedOSPlatform("windows")]
    private static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        builder.Configuration.AddCommandLine(args);
        builder.Configuration.AddEnvironmentVariables();

        var scenarioRouter = new ScenarioRouter(builder.Configuration);
        builder.Services.AddSingleton(scenarioRouter);
        builder.UseTableCloth3SharedComponents();

        switch (scenarioRouter.GetScenario())
        {
            default:
            case Scenario.Launcher:
                builder.UseTableCloth3LauncherComponents();
                break;

            case Scenario.Spork:
                builder.UseTableCloth3SporkComponents();
                break;

            case Scenario.Help:
                break;
        }

        builder.Services.AddAvaloniauiDesktopApplication<App>(BuildAvaloniaApp);

        var app = builder.Build();
        app.RunAvaloniauiApplication(args).GetAwaiter().GetResult();
    }

    // This method is used by both AppHost Avalonia runtime and the Avalonia Designer.
    private static AppBuilder BuildAvaloniaApp(AppBuilder? app)
    {
        if (app == null)
            app = AppBuilder.Configure<App>();

        return app
            .UsePlatformDetect()
            .LogToTrace();
    }

    // This method is required for use with the Avalonia designer.
    // The Avalonia designer will look for this method regardless of whether or not it is private.
    private static AppBuilder BuildAvaloniaApp()
        => BuildAvaloniaApp(null);
}
