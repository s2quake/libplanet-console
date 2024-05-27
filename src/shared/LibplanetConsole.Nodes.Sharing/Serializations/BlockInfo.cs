using Libplanet.Crypto;
using Libplanet.Types.Blocks;

namespace LibplanetConsole.Nodes.Serializations;

public sealed record class BlockInfo
{
    public BlockInfo(Block block)
    {
        Index = block.Index;
        Hash = block.Hash;
        Miner = block.Miner;
        Transactions = block.Transactions.Select(item => new TransactionInfo(item)).ToArray();
    }

    public BlockInfo()
    {
    }

    public long Index { get; init; }

    public BlockHash Hash { get; init; }

    public Address Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
