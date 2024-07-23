using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct TransactionInfo
{
    public TransactionInfo(TxExecution execution, ITransaction transaction)
    {
        Id = (AppId)transaction.Id;
        Signer = (AppAddress)transaction.Signer;
        Actions = transaction.Actions.Select(item => new ActionInfo(item)).ToArray();
        IsFailed = execution.Fail;
    }

    public TransactionInfo()
    {
    }

    public AppId Id { get; init; } = default;

    public AppAddress Signer { get; init; } = default;

    public ActionInfo[] Actions { get; init; } = [];

    public bool IsFailed { get; init; }
}
