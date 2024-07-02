namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct BlockChainInfo
{
    public BlockChainInfo()
    {
    }

    public string GenesisHash { get; init; } = string.Empty;
}
