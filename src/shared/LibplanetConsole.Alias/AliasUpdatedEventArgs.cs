namespace LibplanetConsole.Alias;

public sealed class AliasUpdatedEventArgs(string alias, AliasInfo aliasInfo) : EventArgs
{
    public string Alias { get; } = alias;

    public AliasInfo AliasInfo { get; } = aliasInfo;
}
