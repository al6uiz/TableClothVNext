using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth2.Contracts;
using TableCloth2.Services;
using TableCloth2.Shared.Services;
using TableCloth2.ViewModels;

namespace TableCloth2;

public static class TableCloth2Configurator
{
    public static void ConfigureTableCloth2(this HostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMessageBoxService, MessageBoxService>();

        builder.Services.AddTableClothHttpClient();
        builder.Services.AddChromeLikeHttpClient();

        builder.Services.AddSingleton<KnownPathsService>();
        builder.Services.AddSingleton<ProcessUtility>();
        builder.Services.AddSingleton<Configurations>();
        builder.Services.AddSingleton<SettingsService>();

        builder.Services.AddTransient<BootstrapViewModel>();
        builder.Services.AddTransient<BootstrapForm>();

        builder.Services.AddSingleton<IMessenger, WeakReferenceMessenger>();
    }

    public static IServiceProvider CreateOuterApplication(string[] args)
    {
        var serviceCollection = new ServiceCollection();

        var config = new ConfigurationBuilder()
            .AddCommandLine(args)
            .AddEnvironmentVariables()
            .Build();
        serviceCollection.AddSingleton<IConfiguration>(config);

        serviceCollection.AddSingleton<KnownPathsService>();
        serviceCollection.AddSingleton<Configurations>();
        serviceCollection.AddSingleton<ProcessUtility>();
        return serviceCollection.BuildServiceProvider();
    }
}
