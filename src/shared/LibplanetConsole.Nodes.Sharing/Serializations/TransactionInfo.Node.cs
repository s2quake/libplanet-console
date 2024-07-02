#if LIBPLANET_NODE
using Libplanet.Types.Tx;

namespace LibplanetConsole.Nodes.Serializations;

public readonly partial record struct TransactionInfo
{
    public TransactionInfo(ITransaction transaction)
    {
        Actions = transaction.Actions.Select(item => new ActionInfo(item)).ToArray();
    }
}
#endif // LIBPLANET_NODE
