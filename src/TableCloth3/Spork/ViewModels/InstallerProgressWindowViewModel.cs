using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using TableCloth3.Shared.ViewModels;

namespace TableCloth3.Spork.ViewModels;

public sealed partial class InstallerProgressWindowViewModel : BaseViewModel
{
    [ActivatorUtilitiesConstructor]
    public InstallerProgressWindowViewModel(
        IMessenger messenger)
        : this()
    {
        _messenger = messenger;
    }

    public InstallerProgressWindowViewModel()
        : base()
    {
    }

    private readonly IMessenger _messenger = default!;

    public sealed record class CancelNotification(bool dueToError, Exception? foundException);

    public interface ICancelNotificationRecipient : IRecipient<CancelNotification>;

    public sealed record class FinishNotification;

    public interface IFinishNotificationRecipient : IRecipient<FinishNotification>;

    protected override void PrepareDesignTimePreview()
    {
        for (var i = 0; i < 100; i++)
        {
            var progress = (StepProgress)(i % Enum.GetValues<StepProgress>().Count());
            Steps.Add(new()
            {
                StepProgress = progress,
                PackageName = $"Item {i+1}",
                PackageUrl = "https://yourtablecloth.app/",
                PackageArguments = "/S",
                StepError = progress == StepProgress.Failed ? "An error occurred while processing this step." : string.Empty,
                Percentage = Random.Shared.Next(0, 100),
            });
        }
    }

    [ObservableProperty]
    private ObservableCollection<InstallerStepItemViewModel> _steps = [];

    [RelayCommand]
    private async Task Loaded(CancellationToken cancellationToken = default)
    {
        if (Design.IsDesignMode)
            return;

        await RunInstallerStepsAsync(cancellationToken).ConfigureAwait(false);
    }

    [RelayCommand]
    private void CancelButton()
        => _messenger.Send<CancelNotification>(new(false, default));

    private async Task RunInstallerStepsAsync(CancellationToken cancellationToken = default)
    {
        var loadQueue = new Queue<InstallerStepItemViewModel>();
        var installQueue = new Queue<InstallerStepItemViewModel>();

        try
        {
            foreach (var eachStep in Steps)
                loadQueue.Enqueue(eachStep);

            var loadTask = Task.Run(async () =>
            {
                while (loadQueue.Count > 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;
                    var item = loadQueue.Dequeue();
                    try
                    {
                        item.StepProgress = StepProgress.Loading;
                        await item.LoadInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                        installQueue.Enqueue(item);
                        item.Event.Set();
                        item.StepProgress = StepProgress.Ready;
                    }
                    catch (Exception ex)
                    {
                        item.StepError = ex.Message;
                        item.StepProgress = StepProgress.Failed;
                        _messenger.Send<CancelNotification>(new(true, ex));
                    }
                }
            }, cancellationToken);

            var installTask = Task.Run(async () =>
            {
                var metEndOfSuite = false;

                while (!metEndOfSuite)
                {
                    if (cancellationToken.IsCancellationRequested)
                        break;

                    if (!installQueue.TryDequeue(out var item))
                    {
                        await Task.Delay(TimeSpan.FromSeconds(0.5d), cancellationToken).ConfigureAwait(false);
                        continue;
                    }

                    using (item)
                    {
                        await item.Event.WaitAsync(cancellationToken).ConfigureAwait(false);

                        if (item.ItemType == ItemType.EndOfSuite)
                        {
                            metEndOfSuite = true;
                            continue;
                        }
                        else
                        {
                            try
                            {
                                item.StepProgress = StepProgress.Installing;
                                await item.PerformInstallStepCommand.ExecuteAsync(cancellationToken).ConfigureAwait(false);
                                item.StepProgress = StepProgress.Succeed;
                            }
                            catch (Exception ex)
                            {
                                item.StepError = ex.Message;
                                item.StepProgress = StepProgress.Failed;
                                _messenger.Send<CancelNotification>(new(true, ex));
                            }
                        }
                    }
                }
            }, cancellationToken);

            await Task.WhenAll(loadTask, installTask).ConfigureAwait(false);
            _messenger.Send<FinishNotification>();
        }
        catch (Exception ex)
        {
            _messenger.Send<CancelNotification>(new(true, ex));
        }
    }
}
