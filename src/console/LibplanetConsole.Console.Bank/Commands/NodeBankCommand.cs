using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class NodeBankCommand(
    NodeCommand nodeCommand,
    INodeCollection nodes,
    ICurrencyCollection currencies,
    IAddressCollection addresses)
    : CommandMethodBase(nodeCommand, "bank")
{
    [CommandProperty("node", 'N', InitValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string NodeAddress { get; set; } = string.Empty;

    [CommandProperty(InitValue = "")]
    [CommandSummary("Specifies the address")]
    [CommandPropertyCompletion(nameof(GetAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task TransferAsync(
        [CommandParameterCompletion(nameof(GetAddresses))]
        string recipientAddress,
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        var recipientValue = addresses.ToAddress(recipientAddress);
        await bank.TransferAsync(
            recipientValue, amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyCodes))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var address = NodeAddress == string.Empty ? node.Address : addresses.ToAddress(Address);
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var currencyValue = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currencyValue, cancellationToken);
        await Out.WriteLineAsJsonAsync(currencies.ToString(balance), cancellationToken);
    }

    [CommandMethod]
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

    private string[] GetNodeAddresses() =>
    [
        .. nodes.Where(item => item.Alias != string.Empty).Select(item => item.Alias),
        .. nodes.Select(item => item.Address.ToString())
    ];

    private string[] GetCurrencyCodes() => currencies.Codes;

    private string[] GetAddresses() => addresses.Aliases;
}
