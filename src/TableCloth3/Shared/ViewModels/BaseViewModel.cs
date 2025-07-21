using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace TableCloth3.Shared.ViewModels;

public abstract partial class BaseViewModel : ObservableObject
{
    public BaseViewModel()
        : base()
    {
        if (Design.IsDesignMode)
            PrepareDesignTimePreview();
    }

    protected virtual void PrepareDesignTimePreview() { }

    public virtual void ImportFromModel(object? model) { }

    public virtual void ExportToModel(object? model) { }
}
