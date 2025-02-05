namespace LibplanetConsole.Alias;

public sealed class AliasRemovedEventArgs(string alias) : EventArgs
{
    public string Alias { get; } = alias;
}
