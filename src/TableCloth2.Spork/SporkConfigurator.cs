using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth2.Shared.Contracts;
using TableCloth2.Spork.Services;
using TableCloth2.Spork.ViewModels;
using WindowsFormsLifetime;

namespace TableCloth2.Spork;

public static class SporkConfigurator
{
    public static void ConfigureSpork(this HostApplicationBuilder builder)
    {
        var winFormConfigure = (WindowsFormsLifetimeOptions o) =>
        {
            o.EnableVisualStyles = true;
            o.CompatibleTextRenderingDefault = false;
            o.HighDpiMode = HighDpiMode.SystemAware;
            o.EnableConsoleShutdown = true;
        };

        builder.Services.AddSingleton<IBootstrapper, SporkBootstrapper>();

        builder.Services.AddSingleton<SporkConfigurations>();

        builder.Services.AddTransient<InstallerViewModel>();
        builder.Services.AddTransient<InstallerForm>();

        builder.Services.AddTransient<SporkViewModel>();
        builder.UseWindowsFormsLifetime<SporkForm>(winFormConfigure);
    }
}
