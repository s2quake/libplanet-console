using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(Bank bank) : CommandMethodBase
{
    [CommandMethod]
    public async Task MintAsync(
        Address address,
        [FungibleAssetValue]
        string amount,
        CancellationToken cancellationToken)
    {
        var amountValue = bank.ParseFungibleAssetValue(amount);
        var balance = await bank.MintAsync(address, amountValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance, cancellationToken);
    }

    [CommandMethod]
    public async Task TransferAsync(
        Address address,
        Address targetAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var amountValue = bank.ParseFungibleAssetValue(amount);
        var balance = await bank.TransferAsync(
            address, targetAddress, amountValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance, cancellationToken);
    }

    [CommandMethod]
    public async Task BurnAsync(
        Address address,
        [FungibleAssetValue]
        string amount,
        CancellationToken cancellationToken)
    {
        var amountValue = bank.ParseFungibleAssetValue(amount);
        var balance = await bank.BurnAsync(address, amountValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance, cancellationToken);
    }

    [CommandMethod]
    public async Task BalanceAsync(
        Address address, string currency, CancellationToken cancellationToken)
    {
        var currencyValue = bank.GetCurrency(currency);
        var balance = await bank.GetBalanceAsync(address, currencyValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(balance.ToString(), cancellationToken);
    }

    [CommandMethod]
    public async Task CurrencyAsync(CancellationToken cancellationToken)
    {
        var currencies = await bank.GetCurrenciesAsync(cancellationToken);
        var currencyNames = currencies.Select(item => item.Name).ToArray();
        await Out.WriteLineAsJsonAsync(currencyNames, cancellationToken);
    }
}
