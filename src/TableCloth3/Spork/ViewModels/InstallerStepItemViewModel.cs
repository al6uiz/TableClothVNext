using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TableCloth3.Shared;
using TableCloth3.Shared.Contracts;
using TableCloth3.Shared.Services;
using TableCloth3.Shared.ViewModels;
using TableCloth3.Spork.Services;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerStepItemViewModel : BaseViewModel, IProgress<int>
{
    public sealed record class ProgressNotificationMessage(int? progress);

    public interface IProgressNotificationRecipient : IRecipient<ProgressNotificationMessage>;

    [ActivatorUtilitiesConstructor]
    public InstallerStepItemViewModel(
        LocationService sporkLocationService,
        IHttpClientFactory httpClientFactory,
        IProcessManagerFactory processManagerFactory)
        : this()
    {
        _sporkLocationService = sporkLocationService;
        _httpClientFactory = httpClientFactory;
        _processManagerFactory = processManagerFactory;
    }

    public InstallerStepItemViewModel()
        : base()
    {
    }

    private readonly LocationService _sporkLocationService = default!;
    private readonly IHttpClientFactory _httpClientFactory = default!;
    private readonly IProcessManagerFactory _processManagerFactory = default!;

    [ObservableProperty]
    private bool _isVisible = true;

    [ObservableProperty]
    private string _serviceId = string.Empty;

    [ObservableProperty]
    private string _packageName = string.Empty;

    [ObservableProperty]
    private string _packageUrl = string.Empty;

    [ObservableProperty]
    private string _packageArguments = string.Empty;

    [ObservableProperty]
    private string _stepError = string.Empty;

    [ObservableProperty]
    private StepProgress _stepProgress = StepProgress.None;

    [ObservableProperty]
    private string _localFilePath = string.Empty;

    [ObservableProperty]
    private int _percentage = 0;

    partial void OnStepErrorChanged(string value)
        => OnPropertyChanged(nameof(HasError));

    partial void OnStepProgressChanged(StepProgress value)
    {
        OnPropertyChanged(nameof(StatusText));
        OnPropertyChanged(nameof(ShowPercentage));
    }

    partial void OnPercentageChanged(int value)
        => OnPropertyChanged(nameof(ShowPercentage));

    [RelayCommand]
    private async Task LoadInstallStep(CancellationToken cancellationToken = default)
    {
        Report(0);

        if (!string.IsNullOrWhiteSpace(PackageUrl))
        {
            var tempFileName = $"{ServiceId}_{PackageName.Replace(" ", "_")}";
            var extension = string.Empty;

            if (Uri.TryCreate(PackageUrl, UriKind.Absolute, out var parsedUri) && parsedUri != null)
                extension = Path.GetExtension(parsedUri.LocalPath).TrimStart('.');

            if (string.IsNullOrWhiteSpace(extension))
            {
                if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                    extension = "exe";
                else
                    extension = "bin";
            }

            var filePath = Path.Combine(
                _sporkLocationService.EnsureDownloadsDirectoryCreated().FullName,
                $"{tempFileName}.{extension}");

            var client = _httpClientFactory.CreateChromeHttpClient();
            using var remoteStream = await client.GetStreamAsync(PackageUrl, cancellationToken).ConfigureAwait(false);
            using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);

            var remoteLength = default(long?);
            try { remoteLength = remoteStream.Length; }
            catch { remoteLength = default; }

            Report(30);

            await remoteStream.CopyToAsync(fileStream, remoteLength, cancellationToken: cancellationToken).ConfigureAwait(false);
            LocalFilePath = filePath;
        }

        Report(60);
    }

    [RelayCommand]
    private async Task PerformInstallStep(CancellationToken cancellationToken = default)
    {
        Report(60);

        if (!string.IsNullOrWhiteSpace(LocalFilePath) && File.Exists(LocalFilePath))
        {
            using var processManager = _processManagerFactory.Create();
            await processManager.StartAsync(
                LocalFilePath,
                PackageArguments,
                cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            await processManager.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
        }
        else
            throw new Exception($"Unexpected error: Local file path is empty or does not exist -- {LocalFilePath}");

        Report(100);
    }

    public string StatusText => StepProgress switch
    {
        StepProgress.Loading => "\u23f3",
        StepProgress.Ready => "\ud83d\udce6",
        StepProgress.Installing => "\ud83d\udee0\ufe0f",
        StepProgress.Succeed => "\u2714\ufe0f",
        StepProgress.Failed => "\u274c",
        StepProgress.Unknown => "\u2754",
        _ => "\u2b1c",
    };

    public bool HasError => !string.IsNullOrWhiteSpace(StepError);

    public bool ShowPercentage => StepProgress is StepProgress.Installing or StepProgress.Loading;

    public void Report(int value)
    {
        Percentage = value;
    }
}
