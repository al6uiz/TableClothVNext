﻿using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Launcher.Windows;
using TableCloth3.Shared.Models;
using TableCloth3.Spork.Windows;

namespace TableCloth3.Shared.Services;

public sealed class AvaloniaWindowManager
{
    public AvaloniaWindowManager(
        IServiceProvider serviceProvider,
        ScenarioRouter scenarioRouter)
    {
        _serviceProvider = serviceProvider;
        _scenarioRouter = scenarioRouter;
    }

    private readonly IServiceProvider _serviceProvider;
    private readonly ScenarioRouter _scenarioRouter;

    public TWindow GetAvaloniaWindow<TWindow>()
        where TWindow : Window
        => _serviceProvider.GetRequiredService<TWindow>();

    public Window GetMainAvaloniaWindow()
        => _scenarioRouter.GetScenario() switch
        {
            Scenario.Spork => GetAvaloniaWindow<SporkMainWindow>(),
            _ => GetAvaloniaWindow<LauncherMainWindow>(),
        };
}
