using Libplanet.Types.Tx;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public readonly record struct TransactionInfo
{
    public TransactionInfo(ITransaction transaction)
    {
        Id = (AppId)transaction.Id;
        Signer = (AppAddress)transaction.Signer;
        Actions = transaction.Actions.Select(item => new ActionInfo(item)).ToArray();
    }

    public TransactionInfo()
    {
    }

    public AppId Id { get; init; } = default;

    public AppAddress Signer { get; init; } = default;

    public ActionInfo[] Actions { get; init; } = [];
}
