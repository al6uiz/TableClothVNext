using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Help.ViewModels;

namespace TableCloth3.Help;

internal static class HelpHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3HelpComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<HelpMainWindowViewModel>();
        builder.Services.AddTransient<HelpMainWindow>();

        return builder;
    }
}
