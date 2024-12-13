using Libplanet.Action.State;
using LibplanetConsole.Bank.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(
    IServiceProvider serviceProvider,
    INode node,
    IBlockChain blockChain,
    CurrencyCollection currencies)
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

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var currencyProviders = serviceProvider.GetServices<ICurrencyProvider>();
        var items = currencyProviders.SelectMany(provider => provider.Currencies);
        foreach (var item in items)
        {
            currencies.Add(item.Code, item.Currency);
        }

        await Task.CompletedTask;
    }

    protected override async Task OnStopAsync(CancellationToken cancellationToken)
    {
        currencies.Clear();
        await Task.CompletedTask;
    }
}
