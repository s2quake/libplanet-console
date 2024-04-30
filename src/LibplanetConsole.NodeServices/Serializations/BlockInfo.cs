using Libplanet.Types.Blocks;

namespace LibplanetConsole.NodeServices.Serializations;

public record class BlockInfo
{
    public BlockInfo(Block block)
    {
        Index = block.Index;
        Hash = block.Hash.ToString();
        Miner = block.Miner.ToString();
        Transactions = block.Transactions.Select(item => new TransactionInfo(item)).ToArray();
    }

    public BlockInfo()
    {
    }

    public long Index { get; init; }

    public string Hash { get; init; } = string.Empty;

    public string Miner { get; init; } = string.Empty;

    public TransactionInfo[] Transactions { get; init; } = [];
}
