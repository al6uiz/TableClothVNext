using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Shared.Contracts;
using TableCloth3.Shared.Services;

namespace TableCloth3.Help;

internal static class HelpHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3HelpComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IInitializationService, EmptyInitializationService>();
        return builder;
    }
}
