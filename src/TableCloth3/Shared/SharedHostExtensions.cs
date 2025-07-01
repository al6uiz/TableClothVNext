using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Shared.ViewModels;
using TableCloth3.Shared.Windows;

namespace TableCloth3.Shared;

internal static class SharedHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3SharedComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddTransient<AboutWindowViewModel>();
        builder.Services.AddTransient<AboutWindow>();

        return builder;
    }
}
