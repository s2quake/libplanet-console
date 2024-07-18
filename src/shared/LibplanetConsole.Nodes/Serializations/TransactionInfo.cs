using Libplanet.Types.Tx;

namespace LibplanetConsole.Nodes.Serializations;

public readonly record struct TransactionInfo
{
    public TransactionInfo(ITransaction transaction)
    {
        Actions = transaction.Actions.Select(item => new ActionInfo(item)).ToArray();
    }

    public TransactionInfo()
    {
    }

    public ActionInfo[] Actions { get; init; } = [];
}
