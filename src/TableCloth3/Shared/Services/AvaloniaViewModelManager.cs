using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;

namespace TableCloth3.Shared.Services;

public sealed class AvaloniaViewModelManager
{
    public AvaloniaViewModelManager(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    private readonly IServiceProvider _serviceProvider = default!;

    public TViewModel GetAvaloniaViewModel<TViewModel>()
        where TViewModel : ObservableObject
        => _serviceProvider.GetRequiredService<TViewModel>();
}
