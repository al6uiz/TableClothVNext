using Microsoft.Extensions.Logging;
using System.IO.Compression;
using System.Threading;
using TableCloth2.Services;
using TableCloth2.Shared.Contracts;
using TableCloth2.Shared.Models;
using TableCloth2.Shared.Services;

namespace TableCloth2.TableCloth.Services;

public sealed class TableClothBootstrapper : IBootstrapper
{
    public TableClothBootstrapper(
        ILogger<TableClothBootstrapper> logger,
        SessionService sessionService,
        IHttpClientFactory httpClientFactory,
        KnownPathsService knownPathService,
        ProcessUtility processUtility)
    {
        _logger = logger;
        _sessionService = sessionService;
        _httpClientFactory = httpClientFactory;
        _knownPathService = knownPathService;
        _processUtility = processUtility;
    }

    private readonly ILogger _logger;
    private readonly SessionService _sessionService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly KnownPathsService _knownPathService;
    private readonly ProcessUtility _processUtility;

    public async Task<BootstrapResult> PerformBootstrapAsync(CancellationToken cancellationToken = default)
    {
        var result = new BootstrapResult();

        try
        {
            if (!_sessionService.IsNewInstance)
                throw new Exception("TableCloth is already running.");

            var execDirectory = _processUtility.GetExecutableFileDirectory();
            var settingsDirectory = _knownPathService.EnsureTableClothSettingsDirectoryExists();
            var httpClient = _httpClientFactory.GetTableClothHttpClient();

            // 카탈로그 다운로드
            await DownloadOrCopyAsync(
                "/TableClothCatalog/Catalog.xml",
                execDirectory.Combine("Catalog.xml"),
                settingsDirectory.Combine("Catalog.xml"),
                cancellationToken).ConfigureAwait(false);

            // 이미지 파일 다운로드
            await DownloadOrCopyAsync(
                "/TableClothCatalog/Images.zip",
                execDirectory.Combine("Images.zip"),
                settingsDirectory.Combine("Images.zip"),
                cancellationToken).ConfigureAwait(false);

            // 이미지 파일 압축 해제
            await ExpandArchiveAsync(
                settingsDirectory.Combine("Images.zip"),
                settingsDirectory.Combine("Images"),
                cancellationToken).ConfigureAwait(false);

            result.IsSuccessful = true;
            result.ErrorMessage = null;
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = $"An error occurred while bootstrapping TableCloth: {ex.Message}";
            _logger.LogError(ex, "An error occurred while bootstrapping TableCloth.");
        }

        return result;
    }

    private async Task ExpandArchiveAsync(string zipFilePath, string destinationDirectoryPath, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(destinationDirectoryPath);

        using var zipStream = File.OpenRead(zipFilePath);
        using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Read);

        foreach (var eachEntry in zipArchive.Entries)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(eachEntry.Name))
                    continue;

                var destPath = Path.Combine(destinationDirectoryPath, eachEntry.Name);

                using var outputStream = File.OpenWrite(destPath);
                using var eachStream = eachEntry.Open();
                await eachStream.CopyToAsync(outputStream, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                throw new IOException($"Cannot extract the file '{eachEntry.Name}' to '{destinationDirectoryPath}'.", ex);
            }
        }
    }

    private async Task DownloadOrCopyAsync(string remoteRelativePath, string localAltFileName, string destinationFilePath, CancellationToken cancellationToken = default)
    {
        try
        {
            // Download First
            using var remoteStream = await _httpClientFactory.GetTableClothHttpClient().GetStreamAsync(remoteRelativePath).ConfigureAwait(false);
            using var localStream = File.Open(destinationFilePath, FileMode.Create);
            await remoteStream.CopyToAsync(localStream).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Cannot download the file '{remoteRelativePath}'; Copy local file instead.", remoteRelativePath);

            if (!File.Exists(localAltFileName))
                throw new FileNotFoundException("Cannot find the local file to copy.", localAltFileName);

            // Copy Local File
            using var catalogStream = File.OpenRead(localAltFileName);
            using var fileStream = File.Open(destinationFilePath, FileMode.Create);
            await catalogStream.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
        }
    }
}
