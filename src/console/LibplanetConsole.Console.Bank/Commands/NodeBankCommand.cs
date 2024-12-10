using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class NodeBankCommand(
    NodeCommand nodeCommand, INodeCollection nodes, IBank centralBank)
    : CommandMethodBase(nodeCommand, "bank")
{
    private string[]? _codes;

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
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
        var currencies = bank.Currencies;
        var currencyValue = currencies[currencyCode];
        var balance = await bank.GetBalanceAsync(currencyValue, cancellationToken);
        await Out.WriteLineAsync(currencies.ToString(balance));
    }

    [CommandMethod]
    public void Currency(
        [CommandParameterCompletion(nameof(GetCurrencyAliases))]
        string code = "")
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var bank = node.GetRequiredKeyedService<INodeBank>(INode.Key);
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

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];

    private string[] GetCurrencyAliases() => _codes ??= centralBank.Currencies.Aliases;
}
