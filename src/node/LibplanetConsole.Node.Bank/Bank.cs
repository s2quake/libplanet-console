using System.Text.RegularExpressions;
using Libplanet.Action.State;
using LibplanetConsole.Bank.Actions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(IServiceProvider serviceProvider, IBlockChain blockChain)
    : NodeContentBase(nameof(Bank)), IBank
{
    private readonly Dictionary<string, Currency> _currencyInfos = [];

    public Currency GetCurrency(string name) => _currencyInfos[name];

    public FungibleAssetValue ParseFungibleAssetValue(string text)
    {
        var match = Regex.Match(text, @"(?<value>\d+(\.\d+)?)(?<key>\w+)");
        if (match.Success is false)
        {
            throw new ArgumentException("Invalid format.");
        }

        var key = match.Groups["key"].Value;
        var value = match.Groups["value"].Value;
        if (_currencyInfos.TryGetValue(key, out var currency))
        {
            return FungibleAssetValue.Parse(currency, value);
        }

        throw new ArgumentException("Invalid currency.");
    }

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

    public Task<CurrencyInfo[]> GetCurrenciesAsync(CancellationToken cancellationToken)
        => Task.FromResult(_currencyInfos.Select(GetCurrencyInfo).ToArray());

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        var currencyProviders = serviceProvider.GetServices<ICurrencyProvider>();
        foreach (var currencyProvider in currencyProviders)
        {
            _currencyInfos.Add(currencyProvider.Name, currencyProvider.Currency);
        }

        return Task.CompletedTask;
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        _currencyInfos.Clear();
        return Task.CompletedTask;
    }

    private static CurrencyInfo GetCurrencyInfo(KeyValuePair<string, Currency> keyValuePair)
        => new()
        {
            Name = keyValuePair.Key,
            Currency = keyValuePair.Value,
        };
}
