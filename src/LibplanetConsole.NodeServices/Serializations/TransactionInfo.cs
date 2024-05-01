using Libplanet.Types.Tx;

namespace LibplanetConsole.NodeServices.Serializations;

public record class TransactionInfo
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
