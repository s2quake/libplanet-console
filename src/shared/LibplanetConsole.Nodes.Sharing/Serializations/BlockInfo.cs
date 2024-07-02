using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct BlockInfo
{
    public BlockInfo()
    {
    }

    public long Index { get; init; }

    public AppHash Hash { get; init; }

    public AppAddress Miner { get; init; }

    public TransactionInfo[] Transactions { get; init; } = [];
}
