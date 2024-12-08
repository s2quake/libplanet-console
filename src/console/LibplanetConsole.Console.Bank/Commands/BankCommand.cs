using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(
    IBank bank, INodeCollection nodes, IClientCollection clients)
    : CommandMethodBase
{
    private string[]? _codes;

    [CommandMethod]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        Address recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var currencies = bank.Currencies;
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await bank.TransferAsync(
            recipientAddress, amountValue, cancellationToken);
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string code = "")
    {
        var currencies = bank.Currencies;
        if (code == string.Empty)
        {
            var currencyAliases = currencies.Aliases;
            Out.WriteLineAsJson(currencyAliases);
        }
        else
        {
            var currency = currencies[code];
            Out.WriteLineAsJson(currency);
        }
    }

    [CommandMethod]
    public async Task BalanceAsync(string currency, CancellationToken cancellationToken)
    {
        var currencies = bank.Currencies;
        var currencyValue = currencies[currency];
        var balance = await bank.GetBalanceAsync(currencyValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
    public async Task AllocateAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        Address recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var currencies = bank.Currencies;
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await bank.AllocateAsync(
            recipientAddress, amountValue, cancellationToken);
    }

    [CommandMethod]
    public async Task PoolAsync(string currency, CancellationToken cancellationToken)
    {
        var currencies = bank.Currencies;
        var currencyValue = currencies[currency];
        var balance = await bank.GetPoolAsync(currencyValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    private string[] GetCurrencyAliases() => _codes ??= bank.Currencies.Aliases;

    private string[] GetAddresses()
    {
        var nodesAddresses = nodes.Select(item => item.Address.ToHex());
        var clientsAddresses = clients.Select(item => item.Address.ToHex());
        return nodesAddresses.Concat(clientsAddresses).ToArray();
    }
}
