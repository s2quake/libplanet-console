namespace LibplanetConsole.Common;

public sealed class ItemEventArgs<T>(T item) : EventArgs
{
    public T Item { get; } = item;
}
