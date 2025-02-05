namespace LibplanetConsole.Alias;

public sealed class AliasEventArgs(AliasInfo aliasInfo) : EventArgs
{
    public AliasInfo AliasInfo { get; } = aliasInfo;
}
