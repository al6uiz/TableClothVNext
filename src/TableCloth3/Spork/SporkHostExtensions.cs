using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Shared.Services;
using TableCloth3.Spork.Services;
using TableCloth3.Spork.ViewModels;
using TableCloth3.Spork.Windows;

namespace TableCloth3.Spork;

internal static class SporkHostExtensions
{
    internal static readonly string CatalogHttpClient = nameof(CatalogHttpClient);

    public static HttpClient CreateCatalogHttpClient(this IHttpClientFactory httpClientFactory)
        => httpClientFactory.CreateClient(CatalogHttpClient);

    public static IHostApplicationBuilder UseTableCloth3SporkComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<AvaloniaWindowManager>();

        builder.Services.AddHttpClient(CatalogHttpClient, client =>
        {
            client.BaseAddress = new Uri("https://yourtablecloth.app", UriKind.Absolute);
        });

        builder.Services.AddTransient<TableClothCatalogService>();

        builder.Services.AddTransient<InstallerProgressWindowViewModel>();
        builder.Services.AddTransient<InstallerStepItemViewModel>();
        builder.Services.AddTransient<InstallerProgressWindow>();

        builder.Services.AddTransient<SporkMainWindowViewModel>();
        builder.Services.AddTransient<TableClothCatalogItemViewModel>();
        builder.Services.AddTransient<TableClothPackageItemViewModel>();
        builder.Services.AddTransient<SporkMainWindow>();

        return builder;
    }
}
