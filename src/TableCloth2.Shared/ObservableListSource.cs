using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TableCloth2.Shared;

// https://learn.microsoft.com/ko-kr/ef/ef6/fundamentals/databinding/winforms
public sealed class ObservableListSource<T> : ObservableCollection<T>, IListSource
    where T : class
{
    private IBindingList? _bindingList;

    bool IListSource.ContainsListCollection => false;

    IList IListSource.GetList() => _bindingList ?? (_bindingList = this.ToBindingList());
}
