using LibplanetConsole.Bank.Actions;
using Microsoft.Extensions.DependencyInjection;
using Nekoyume.Model.State;

namespace LibplanetConsole.Console.Bank;

internal sealed class Bank(INodeCollection nodes, IConsole console, IBlockChain blockChain)
    : ConsoleContentBase("bank"), IBank
{
    public CurrencyCollection Currencies => NodeBank.Currencies;

    private INodeBank NodeBank => nodes.Current switch
    {
        null => throw new InvalidOperationException("Node is not selected."),
        _ => nodes.Current.GetRequiredKeyedService<INodeBank>(INode.Key),
    };

    public async Task TransferAsync(
        Address recipientAddress,
        FungibleAssetValue amount,
        CancellationToken cancellationToken)
    {
        var senderAddress = console.Address;
        var actions = new IAction[]
        {
            new TransferAction(senderAddress, recipientAddress, amount),
        };

        await console.SendTransactionAsync(actions, cancellationToken);
    }

    public async Task<FungibleAssetValue> GetBalanceAsync(
        Currency currency, CancellationToken cancellationToken)
    {
        var address = console.Address;
        return await blockChain.GetBalanceAsync(address, currency, cancellationToken);
    }

    public async Task AllocateAsync(
            Address recipientAddress,
            FungibleAssetValue amount,
            CancellationToken cancellationToken)
    {
        var senderAddress = GoldCurrencyState.Address;
        var actions = new IAction[]
        {
            new TransferAction(senderAddress, recipientAddress, amount),
        };

        await console.SendTransactionAsync(actions, cancellationToken);
    }

    public async Task<FungibleAssetValue> GetPoolAsync(
        Currency currency, CancellationToken cancellationToken)
    {
        var address = GoldCurrencyState.Address;
        return await blockChain.GetBalanceAsync(address, currency, cancellationToken);
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
