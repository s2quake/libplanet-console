using Libplanet.Action.State;
using LibplanetConsole.Bank.Actions;
using LibplanetConsole.BlockChain;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Bank)), IBank
{
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
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        FungibleAssetValue Action()
            => blockChain.GetWorldState().GetBalance(address, currency);

        return await Task.Run(Action, cancellationToken);
    }
}
