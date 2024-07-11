using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly partial record struct TransactionInfo
{
    public TransactionInfo()
    {
    }

    public AppId Id { get; init; } = default;

    public AppAddress Signer { get; init; } = default;

    public ActionInfo[] Actions { get; init; } = [];

    public bool IsFailed { get; init; }

    public long Height { get; init; }
}
