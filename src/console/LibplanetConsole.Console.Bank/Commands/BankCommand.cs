using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(
    IConsole console,
    IBank bank,
    ICurrencyCollection currencies,
    IAddressCollection addresses,
    INodeCollection nodes,
    IClientCollection clients)
    : CommandMethodBase
{
    [CommandProperty(InitValue = "")]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        string recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var amountValue = currencies.ToFungibleAssetValue(amount);
        var recipientValue = addresses.ToAddress(recipientAddress);
        await bank.TransferAsync(
            recipientValue, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var address = Address == string.Empty ? console.Address : addresses.ToAddress(Address);
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currency, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string code = "")
    {
        if (code == string.Empty)
        {
            var currencyCodes = currencies.Codes;
            Out.WriteLineAsJson(currencyCodes);
        }
        else
        {
            var currency = currencies[code];
            Out.WriteLineAsJson(currency);
        }
    }

    private string[] GetCurrencyCodes() => currencies.Codes;

    private string[] GetAddresses()
    {
        return
        [
            .. addresses.Aliases,
            .. nodes.Select(item => item.Address.ToHex()),
            .. clients.Select(item => item.Address.ToHex()),
        ];
    }
}
