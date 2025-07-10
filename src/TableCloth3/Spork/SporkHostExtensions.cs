using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Shared.Services;
using TableCloth3.Spork.Contracts;
using TableCloth3.Spork.Services;
using TableCloth3.Spork.ViewModels;
using TableCloth3.Spork.Windows;

namespace TableCloth3.Spork;

internal static class SporkHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3SporkComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<SporkLocationService>();
        builder.Services.AddTransient<TableClothCatalogService>();

        if (OperatingSystem.IsWindows())
            builder.Services.AddSingleton<IElevationService, WindowsElevationService>();
        else
            builder.Services.AddSingleton<IElevationService, UnixElevationService>();

        builder.Services.AddTransient<InstallerProgressWindowViewModel>();
        builder.Services.AddTransient<InstallerStepItemViewModel>();
        builder.Services.AddTransient<InstallerProgressWindow>();

        builder.Services.AddTransient<SporkMainWindowViewModel>();
        builder.Services.AddTransient<TableClothCatalogItemViewModel>();
        builder.Services.AddTransient<TableClothPackageItemViewModel>();
        builder.Services.AddTransient<TableClothCategoryItemViewModel>();
        builder.Services.AddTransient<SporkMainWindow>();

        return builder;
    }
}
