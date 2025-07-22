using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using TableCloth3.Spork.Contracts;
using TableCloth3.Spork.Services;
using TableCloth3.Spork.ViewModels;
using TableCloth3.Spork.Windows;

namespace TableCloth3.Spork;

internal static class SporkHostExtensions
{
    public static IHostApplicationBuilder UseTableCloth3SporkComponents(this IHostApplicationBuilder builder)
    {
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

    internal static async Task CopyToAsync(
        this Stream source,
        Stream destination,
        long? totalBytes = null,
        IProgress<int>? progress = null,
        int bufferSize = 81920,
        CancellationToken cancellationToken = default)
    {
        if (source is null)
            throw new ArgumentNullException(nameof(source));
        if (!source.CanRead)
            throw new IOException("Selected source stream is not readable.");
        if (!destination.CanWrite)
            throw new IOException("Selected destination stream is not writable.");
        if (destination is null)
            throw new ArgumentNullException(nameof(destination));
        if (bufferSize < 2)
            throw new ArgumentOutOfRangeException(nameof(bufferSize));

        var buffer = new byte[bufferSize];
        var totalRead = 0L;
        var read = 0;

        while ((read = await source.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
        {
            await destination.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
            totalRead += read;

            if (totalBytes.HasValue && totalBytes > 0L)
                progress?.Report((int)Math.Round((double)totalRead / totalBytes.Value));
            else
                progress?.Report(50);
        }

        if (totalBytes.HasValue && totalRead == totalBytes.Value)
            progress?.Report(100);
    }
}
