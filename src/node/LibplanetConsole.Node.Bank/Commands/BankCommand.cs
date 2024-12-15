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
    IAddressCollection addresses) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    public async Task TransferAsync(
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
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var address = Address == string.Empty ? node.Address : new Address(Address);
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currency, cancellationToken);
        await Out.WriteLineAsync(currencies.ToString(balance));
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
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

    private string[] GetCurrencyAliases() => currencies.Codes;

    private string[] GetAddresses() => addresses.Aliases;
}
