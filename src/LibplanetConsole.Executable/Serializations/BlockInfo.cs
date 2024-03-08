using Libplanet.Types.Blocks;

namespace LibplanetConsole.Executable.Serializations;

record class BlockInfo
{
    public BlockInfo(Block block)
    {
        Index = block.Index;
        Hash = block.Hash.ToString();
        Transactions = block.Transactions.Select(item => new TransactionInfo(item)).ToArray();
    }

    public long Index { get; }

    public string Hash { get; }

    public TransactionInfo[] Transactions { get; }
}
