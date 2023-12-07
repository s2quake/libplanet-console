using Libplanet.Types.Tx;

namespace OnBoarding.ConsoleHost.Serializations;

record class TransactionInfo
{
    public TransactionInfo(ITransaction transaction)
    {
        Actions = [.. transaction.Actions.Select(item => new ActionInfo(item))];
    }

    public ActionInfo[] Actions { get; }
}
