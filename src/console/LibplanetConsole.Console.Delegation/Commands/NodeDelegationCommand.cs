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
    [CommandProperty(InitValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string NodeAddress { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task PromoteAsync(
        [FungibleAssetValue] string guildGold,
        CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var value = currencies.ToFungibleAssetValue(guildGold);
        await validator.PromoteAsync(value, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.UnjailAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegateAsync(
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var amountValue = currencies.ToFungibleAssetValue(amount);
        await validator.DelegateAsync(amountValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task UndelegateAsync(
        string share,
        CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var shareValue = BigInteger.Parse(share, System.Globalization.NumberStyles.AllowThousands);
        await delegation.UndelegateAsync(shareValue, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task CommissionAsync(
        long commission,
        CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.SetCommissionAsync(commission, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var address = NodeAddress;
        var node = nodes.GetNodeOrCurrent(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegateeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var targetAddress = address == string.Empty ? node.Address : new Address(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegateeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegatorInfoAsync(
       string address = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var targetAddress = address == string.Empty ? node.Address : new Address(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegatorInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task StakeInfoAsync(
        string address = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var targetAddress = address == string.Empty ? node.Address : new Address(address);
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetStakeInfoAsync(targetAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];
}
