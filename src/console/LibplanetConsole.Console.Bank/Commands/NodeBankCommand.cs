using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class NodeBankCommand(
    NodeCommand nodeCommand, INodeCollection nodes, ICurrencyCollection currencies)
    : CommandMethodBase(nodeCommand, "bank")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string currencyCode,
        CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var address = Address == string.Empty ? node.Address : new Address(Address);
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var currencyValue = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(address, currencyValue, cancellationToken);
        await Out.WriteLineAsync(currencies.ToString(balance));
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
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

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];

    private string[] GetCurrencyAliases() => currencies.Codes;
}
