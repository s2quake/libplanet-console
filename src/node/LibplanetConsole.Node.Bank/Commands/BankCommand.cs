using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Bank.Commands;

[CommandSummary("Bank Commands.")]
[Category("Bank")]
internal sealed class BankCommand(
    INode node,
    IBank bank,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : CommandMethodBase
{
    [CommandProperty]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static Address Address { get; set; }

    [CommandMethod]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        Address recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await bank.TransferAsync(
            recipientAddress, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var address = Address == default ? node.Address : Address;
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

    private string[] GetAddresses() => addresses.Aliases;
}
