using Libplanet.Action.State;
using LibplanetConsole.Bank.Actions;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(IBlockChain blockChain) : NodeContentBase(nameof(Bank)), IBank
{
    public async Task<FungibleAssetValue> MintAsync(
        Address address, FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        var actions = new IAction[]
        {
            new MintAction
            {
                Address = address,
                Amount = amount,
            },
        };
        await blockChain.SendTransactionAsync(actions, cancellationToken);
        return blockChain.GetWorldState().GetBalance(address, amount.Currency);
    }

    public async Task<FungibleAssetValue> TransferAsync(
        Address address,
        Address targetAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        var actions = new IAction[]
        {
            new TransferAction
            {
                Address = address,
                TargetAddress = targetAddress,
                Amount = amount,
            },
        };
        await blockChain.SendTransactionAsync(actions, cancellationToken);
        return blockChain.GetWorldState().GetBalance(address, amount.Currency);
    }

    public async Task<FungibleAssetValue> BurnAsync(
        Address address, FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        var actions = new IAction[]
        {
            new BurnAction
            {
                Address = address,
                Amount = amount,
            },
        };
        await blockChain.SendTransactionAsync(actions, cancellationToken);
        return blockChain.GetWorldState().GetBalance(address, amount.Currency);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Address address, Currency currency, CancellationToken cancellationToken)
    {
        FungibleAssetValue Action()
            => blockChain.GetWorldState().GetBalance(address, currency);

        return await Task.Run(Action);
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;

    protected override Task OnStopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}
