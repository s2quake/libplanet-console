namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct TransactionInfo
{
    public TransactionInfo()
    {
    }

    public ActionInfo[] Actions { get; init; } = [];
}
