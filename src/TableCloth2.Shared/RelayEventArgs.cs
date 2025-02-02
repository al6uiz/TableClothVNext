namespace TableCloth2.Shared;

public sealed class RelayEventArgs<T> : EventArgs
{
    public RelayEventArgs(T initialValue)
    {
        _value = initialValue;
    }

    private T _value;
    private bool _accepted;

    public T Value
    {
        get => _value;
        set => _value = value;
    }

    public bool Accepted
    {
        get => _accepted;
        set => _accepted = value;
    }

    public override string ToString()
    {
        return $"{{ {_value} }}";
    }
}
