﻿using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TableCloth3.Launcher.ViewModels;
using TableCloth3.Launcher.Windows;
using TableCloth3.Shared.Services;

namespace TableCloth3.Launcher;

internal static class LauncherHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3LauncherComponents(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);
        builder.Services.AddSingleton<AvaloniaWindowManager>();

        builder.Services.AddTransient<FolderManageWindowViewModel>();
        builder.Services.AddTransient<FolderManageWindow>();

        builder.Services.AddTransient<LauncherMainWindowViewModel>();
        builder.Services.AddTransient<LauncherMainWindow>();

        return builder;
    }
}
