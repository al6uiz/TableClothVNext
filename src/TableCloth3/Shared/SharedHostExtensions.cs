using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.ViewModels;
using TableCloth3.Shared.Windows;
using TableCloth3.Spork.Services;

namespace TableCloth3.Shared;

internal static class SharedHostExtensions
{
    internal static readonly string CatalogHttpClient = nameof(CatalogHttpClient);

    public static HttpClient CreateCatalogHttpClient(this IHttpClientFactory httpClientFactory)
        => httpClientFactory.CreateClient(CatalogHttpClient);

    public static IHostApplicationBuilder UseTableCloth3SharedComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<AvaloniaViewModelManager>();
        builder.Services.AddSingleton<AvaloniaWindowManager>();

        builder.Services.AddHttpClient(CatalogHttpClient, client =>
        {
            client.BaseAddress = new Uri("https://yourtablecloth.app", UriKind.Absolute);
        });

        builder.Services.AddSingleton<ArchiveExpander>();
        builder.Services.AddTransient<TableClothCatalogService>();

        builder.Services.AddTransient<AboutWindowViewModel>();
        builder.Services.AddTransient<AboutWindow>();

        return builder;
    }
}
