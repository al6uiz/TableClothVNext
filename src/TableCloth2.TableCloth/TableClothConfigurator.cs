using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth2.Shared.Contracts;
using TableCloth2.TableCloth.Contracts;
using TableCloth2.TableCloth.Services;
using TableCloth2.TableCloth.ViewModels;
using WindowsFormsLifetime;

namespace TableCloth2.TableCloth;

public static class TableClothConfigurator
{
    public static void ConfigureTableCloth(this HostApplicationBuilder builder)
    {
        var winFormConfigure = (WindowsFormsLifetimeOptions o) =>
        {
            o.EnableVisualStyles = true;
            o.CompatibleTextRenderingDefault = false;
            o.HighDpiMode = HighDpiMode.SystemAware;
            o.EnableConsoleShutdown = true;
        };

        builder.Services.AddSingleton<IBootstrapper, TableClothBootstrapper>();
        builder.Services.AddSingleton<ISandboxComposer, WindowsSandboxComposer>();

        builder.Services.AddSingleton<SessionService>();
        builder.Services.AddSingleton<SettingsService>();

        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<SettingsForm>();

        builder.Services.AddTransient<TableClothViewModel>();
        builder.UseWindowsFormsLifetime<TableClothForm>(winFormConfigure);
    }
}
