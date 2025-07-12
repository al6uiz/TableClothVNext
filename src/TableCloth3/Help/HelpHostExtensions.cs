using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TableCloth3.Help;

internal static class HelpHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3HelpComponents(this IHostApplicationBuilder builder)
    {
        return builder;
    }
}
