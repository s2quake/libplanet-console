using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(IBank bank) : CommandMethodBase
{
    private string[]? _codes;

    [CommandMethod]
    public async Task TransferAsync(
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
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var currencies = bank.Currencies;
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(currency, cancellationToken);
        await Out.WriteLineAsync(currencies.ToString(balance));
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

    private string[] GetCurrencyAliases() => _codes ??= bank.Currencies.Aliases;
}
