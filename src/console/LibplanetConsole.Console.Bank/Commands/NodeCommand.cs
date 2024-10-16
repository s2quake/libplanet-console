using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Bank.Commands;

internal sealed partial class NodeCommand(INodeCollection nodes) : CommandMethodBase
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    public static Address Address { get; set; }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Mint NCG to specific address.")]
    [Category("Bank")]
    public async Task MintAsync(
        [CommandSummary("The amount of NCG to mint.")]
        decimal amount,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes[address];
        var bank = node.GetRequiredService<IBank>();
        var options = new MintOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bank.MintAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Transfer NCG to specific address.")]
    [Category("Bank")]
    public async Task TransferAsync(
        [CommandSummary("The address of the recipient.")]
        Address targetAddress,
        [CommandSummary("The amount of NCG to transfer.")]
        decimal amount,
        CancellationToken cancellationToken)
    {
        var node = nodes[Address];
        var bank = node.GetRequiredService<IBank>();
        var options = new TransferOptions
        {
            TargetAddress = targetAddress,
            Amount = amount,
        };
        var balanceInfo = await bank.TransferAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Burn NCG.")]
    [Category("Bank")]
    public async Task BurnAsync(
        [CommandSummary("The amount of NCG to burn.")]
        decimal amount,
        CancellationToken cancellationToken)
    {
        var node = nodes[Address];
        var bank = node.GetRequiredService<IBank>();
        var options = new BurnOptions
        {
            Amount = amount,
        };
        var balanceInfo = await bank.BurnAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    [CommandSummary("Display balance of specific address.")]
    [Category("Bank")]
    public async Task BalanceAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes[address];
        var bank = node.GetRequiredService<IBank>();
        var balanceInfo = await bank.GetBalanceAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(balanceInfo);
    }
}
