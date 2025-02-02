namespace TableCloth2.Shared;

public sealed class RelayCommandArgument
{
    public RelayCommandArgument(
        object? sender,
        EventArgs eventArgs)
    {
        Sender = sender;
        EventArgs = eventArgs;
    }

    public object? Sender { get; }

    public EventArgs EventArgs { get; }
}
