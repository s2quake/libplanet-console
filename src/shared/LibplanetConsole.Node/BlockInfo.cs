namespace LibplanetConsole.Node;

public readonly partial record struct BlockInfo
{
    public BlockInfo()
    {
    }

    public long Height { get; init; }

    public BlockHash Hash { get; init; }

    public Address Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
