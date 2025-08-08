using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableCloth3.Spork.Contracts;
using TableCloth3.Spork.Services;
using TableCloth3.Spork.ViewModels;
using TableCloth3.Spork.Windows;

namespace TableCloth3.Lite;

internal static class LiteHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3LiteComponents(this IHostApplicationBuilder builder)
    {
        //if (OperatingSystem.IsWindows())
        //    builder.Services.AddSingleton<IElevationService, WindowsElevationService>();
        //else
        //    builder.Services.AddSingleton<IElevationService, UnixElevationService>();

        //builder.Services.AddTransient<InstallerProgressWindowViewModel>();
        //builder.Services.AddTransient<InstallerStepItemViewModel>();
        //builder.Services.AddTransient<InstallerProgressWindow>();

        //builder.Services.AddTransient<SporkMainWindowViewModel>();
        //builder.Services.AddTransient<TableClothCatalogItemViewModel>();
        //builder.Services.AddTransient<TableClothPackageItemViewModel>();
        //builder.Services.AddTransient<TableClothCategoryItemViewModel>();
        //builder.Services.AddTransient<TableClothAddonItemViewModel>();
        //builder.Services.AddTransient<SporkMainWindow>();

        return builder;
    }
}
