#if LIBPLANET_NODE
using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public readonly partial record struct TransactionInfo
{
    public TransactionInfo(TxExecution execution, ITransaction transaction)
    {
        Id = transaction.Id;
        Signer = transaction.Signer;
        Actions = GetActionInfos(transaction);
        IsFailed = execution.Fail;
    }

    private static ActionInfo[] GetActionInfos(ITransaction transaction)
    {
        var infos = new ActionInfo[transaction.Actions.Count];
        for (var i = 0; i < transaction.Actions.Count; i++)
        {
            var action = transaction.Actions[i];
            infos[i] = new ActionInfo(action) { Index = i, TxId = transaction.Id };
        }

        return infos;
    }
}
#endif // LIBPLANET_NODE
