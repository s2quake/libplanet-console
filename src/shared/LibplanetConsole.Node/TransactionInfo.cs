namespace LibplanetConsole.Node;

public readonly partial record struct TransactionInfo
{
    public TransactionInfo()
    {
    }

    public TxId Id { get; init; } = default;

    public Address Signer { get; init; } = default;

    public ActionInfo[] Actions { get; init; } = [];

    public bool IsFailed { get; init; }

    public long Height { get; init; }
}
