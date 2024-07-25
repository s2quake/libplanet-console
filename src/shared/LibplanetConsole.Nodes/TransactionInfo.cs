using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct TransactionInfo
{
    public TransactionInfo(TxExecution execution, ITransaction transaction)
    {
        Id = (AppId)transaction.Id;
        Signer = (AppAddress)transaction.Signer;
        Actions = GetActionInfos(transaction);
        IsFailed = execution.Fail;
    }

    public TransactionInfo()
    {
    }

    public AppId Id { get; init; } = default;

    public AppAddress Signer { get; init; } = default;

    public ActionInfo[] Actions { get; init; } = [];

    public bool IsFailed { get; init; }

    public long Height { get; init; }

    private static ActionInfo[] GetActionInfos(ITransaction transaction)
    {
        var infos = new ActionInfo[transaction.Actions.Count];
        for (var i = 0; i < transaction.Actions.Count; i++)
        {
            var action = transaction.Actions[i];
            infos[i] = new ActionInfo(action) { Index = i, TxId = (AppId)transaction.Id };
        }

        return infos;
    }
}
