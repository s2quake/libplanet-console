using System.Numerics;
using JSSoft.Commands;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Bank;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Delegation.Commands;

internal sealed partial class NodeDelegationCommand(
    NodeCommand nodeCommand,
    INodeCollection nodes,
    ICurrencyCollection currencies)
    : CommandMethodBase(nodeCommand, "delegation")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task PromoteAsync(
        [FungibleAssetValue] string guildGold,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var value = currencies.ToFungibleAssetValue(guildGold);
        await validator.PromoteAsync(value, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.UnjailAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task DelegateAsync(
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await validator.DelegateAsync(amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task UndelegateAsync(
        string share,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var shareValue = BigInteger.Parse(share, System.Globalization.NumberStyles.AllowThousands);
        await delegation.UndelegateAsync(shareValue, cancellationToken);
    }

    [CommandMethod]
    public async Task CommissionAsync(
        long commission,
        CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.SetCommissionAsync(commission, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(Address))]
    public async Task InfoAsync(CancellationToken cancellationToken)
    {
        var address = Address;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegateeInfoAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];
}
