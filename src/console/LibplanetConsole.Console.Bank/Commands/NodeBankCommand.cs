using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Bank.Commands;

[CommandSummary("Provides bank-related commands for the node.")]
[Category("Bank")]
internal sealed partial class NodeBankCommand(
    IServiceProvider serviceProvider,
    NodeCommand nodeCommand,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : NodeCommandMethodBase(serviceProvider, nodeCommand, "bank")
{
    [CommandProperty]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static Address Address { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Transfers the specified amount to the recipient address.")]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        Address recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await bank.TransferAsync(
            recipientAddress, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Gets the balance of the specified currency for the node or " +
                    "specified address.")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var address = Address == default ? node.Address : Address;
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var currencyValue = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currencyValue, cancellationToken);
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
            var currencyAliases = currencies.Codes;
            Out.WriteLineAsJson(currencyAliases);
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
