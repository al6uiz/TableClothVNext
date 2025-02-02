using Microsoft.Extensions.Logging;
using TableCloth2.Services;
using TableCloth2.Shared.Contracts;
using TableCloth2.Shared.Models;

namespace TableCloth2.Spork.Services;

public sealed class SporkBootstrapper : IBootstrapper
{
    public SporkBootstrapper(
        ILogger<SporkBootstrapper> logger,
        KnownPathsService knownPathsService)
    {
        _logger = logger;
        _knownPathServices = knownPathsService;
    }

    private readonly ILogger _logger;
    private readonly KnownPathsService _knownPathServices;

    public async Task<BootstrapResult> PerformBootstrapAsync(CancellationToken cancellationToken = default)
    {
        var result = new BootstrapResult();

        try
        {
            // 공동 인증서 디렉터리 복사
            var sandboxDesktopNpkiDirectory = _knownPathServices.SandboxDesktopNPKIDirectory;
            var localLowNpkiDirectory = _knownPathServices.EnsureLocalLowNPKIDirectoryExists();
            try { await CopyDirectoryAsync(sandboxDesktopNpkiDirectory, localLowNpkiDirectory, true, cancellationToken).ConfigureAwait(false); }
            catch (Exception ex)
            {
                result.Warnings.Add("Cannot copy the NPKI directory.");
                _logger.LogWarning(ex, "Cannot copy the NPKI directory.");
            }

            // 설정 디렉터리 복사
            var sandboxDesktopSettingsDirectory = _knownPathServices.SandboxDesktopSettingsDirectory;
            var tableclothSettingsDirectory = _knownPathServices.EnsureTableClothSettingsDirectoryExists();
            try { await CopyDirectoryAsync(sandboxDesktopSettingsDirectory, tableclothSettingsDirectory, true, cancellationToken).ConfigureAwait(false); }
            catch (Exception ex)
            {
                result.Warnings.Add("Cannot copy the settings directory.");
                _logger.LogWarning(ex, "Cannot copy the settings directory.");
            }

            result.IsSuccessful = true;
            result.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    private async Task CopyDirectoryAsync(DirectoryInfo sourceDir, DirectoryInfo destinationDir, bool recursive = true, CancellationToken cancellationToken = default)
    {
        // Check if the source directory exists
        if (!sourceDir.Exists)
            throw new DirectoryNotFoundException($"Source directory not found: {sourceDir.FullName}");

        // Cache directories before we start copying
        var dirs = sourceDir.GetDirectories();

        // Create the destination directory
        destinationDir.Create();

        // Get the files in the source directory and copy to the destination directory
        foreach (FileInfo file in sourceDir.GetFiles())
        {
            using var sourceFile = file.Open(FileMode.Open, FileAccess.Read);
            using var destinationFile = File.Open(destinationDir.Combine(file.Name), FileMode.Create);
            await sourceFile.CopyToAsync(destinationFile, cancellationToken).ConfigureAwait(false);
        }

        // If recursive and copying subdirectories, recursively call this method
        if (recursive)
        {
            foreach (var subDir in dirs)
            {
                await CopyDirectoryAsync(
                    subDir,
                    new DirectoryInfo(destinationDir.Combine(subDir.Name)),
                    true, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
