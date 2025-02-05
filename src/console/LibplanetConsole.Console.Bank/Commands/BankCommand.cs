using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Alias;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Provides bank-related commands.")]
[Category("Bank")]
internal sealed class BankCommand(
    IConsole console,
    IBank bank,
    ICurrencyCollection currencies,
    IAliasCollection aliases,
    INodeCollection nodes,
    IClientCollection clients)
    : CommandMethodBase
{
    [CommandProperty]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static Address Address { get; set; }

    [CommandMethod]
    [CommandSummary("Transfers the specified amount to the recipient address.")]
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
    [CommandSummary("Gets the balance of the specified currency for the console or " +
                    "specified address.")]
    [CommandMethodProperty(nameof(Address))]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var address = Address == default ? console.Address : Address;
        var currency = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currency, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Displays information about a specific currency or lists all currency codes.")]
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
            .. aliases.Aliases,
            .. nodes.Select(item => item.Address.ToHex()),
            .. clients.Select(item => item.Address.ToHex()),
        ];
    }
}
