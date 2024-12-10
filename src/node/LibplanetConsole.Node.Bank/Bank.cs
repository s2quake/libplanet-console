using Libplanet.Action.State;
using LibplanetConsole.Bank.Actions;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Bank)), IBank
{
    public CurrencyCollection Currencies { get; private set; } = [];

    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        var senderAddress = node.Address;
        var actions = new IAction[]
        {
            new TransferAction(senderAddress, recipientAddress, amount),
        };
        await node.SendTransactionAsync(actions, cancellationToken);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Currency currency, CancellationToken cancellationToken)
    {
        var address = node.Address;
        FungibleAssetValue Action()
            => blockChain.GetWorldState().GetBalance(address, currency);

        return await Task.Run(Action, cancellationToken);
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
