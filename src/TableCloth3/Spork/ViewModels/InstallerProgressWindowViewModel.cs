using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerProgressWindowViewModel : BaseViewModel
{
    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            Steps.Add(new()
            {
                PackageName = $"Item {i+1}",
                PackageUrl = "https://yourtablecloth.app/",
                PackageArguments = "/S",
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];
}

public sealed partial class InstallerStepItemViewModel : BaseViewModel
{
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

    [RelayCommand]
    private async Task PerformInstallStep(CancellationToken cancellationToken = default)
    {
        try
        {
            StepProgress = StepProgress.Installing;

            await Task.Delay(TimeSpan.FromSeconds(1d), cancellationToken).ConfigureAwait(false);

            StepProgress = StepProgress.Succeed;
        }
        catch (Exception ex)
        {
            StepError = ex.Message;
            StepProgress = StepProgress.Failed;
        }
    }
}

public enum StepProgress
{
    None,
    Ready,
    Installing,
    Succeed,
    Failed,
    Unknown,
}
