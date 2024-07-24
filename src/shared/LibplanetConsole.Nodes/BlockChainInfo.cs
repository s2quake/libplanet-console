using Libplanet.Blockchain;

namespace LibplanetConsole.Nodes;

public readonly record struct BlockChainInfo
{
    public BlockChainInfo(BlockChain blockChain)
    {
        GenesisHash = $"{blockChain.Genesis.Hash}";
    }

    public BlockChainInfo()
    {
    }

    public string GenesisHash { get; init; } = string.Empty;
}
