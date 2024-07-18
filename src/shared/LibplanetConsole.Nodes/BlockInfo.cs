using Libplanet.Types.Blocks;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct BlockInfo
{
    public BlockInfo()
    {
    }

    internal BlockInfo(Block block)
    {
        Index = block.Index;
        Hash = (AppHash)block.Hash;
        Miner = (AppAddress)block.Miner;
        Transactions = block.Transactions.Select(item => new TransactionInfo(item)).ToArray();
    }

    public long Index { get; init; }

    public AppHash Hash { get; init; }

    public AppAddress Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
