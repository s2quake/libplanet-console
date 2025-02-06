using System.Numerics;
using JSSoft.Commands;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Delegation.Commands;

internal sealed partial class NodeDelegationCommand(
    IServiceProvider serviceProvider,
    NodeCommand nodeCommand,
    ICurrencyCollection currencies)
    : NodeCommandMethodBase(serviceProvider, nodeCommand, "delegation")
{
    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task StakeAsync(
        long ncg,
        CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.StakeAsync(ncg, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task PromoteAsync(
        [FungibleAssetValue] string guildGold,
        CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var value = currencies.ToFungibleAssetValue(guildGold);
        await validator.PromoteAsync(value, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var validator = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await validator.UnjailAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegateAsync(
        [FungibleAssetValue] string amount,
        CancellationToken cancellationToken)
    {
        var node = CurrentNode;
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
        var node = CurrentNode;
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
        var node = CurrentNode;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.SetCommissionAsync(commission, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task SlashAsync(
        long slashFactor = 10,
        CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        await delegation.SlashAsync(slashFactor, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegateeInfoAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var actualAddress = address == default ? node.Address : address;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegateeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DelegatorInfoAsync(
        [CommandParameterCompletion(nameof(GetDelegatorAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var actualAddress = address == default ? node.Address : address;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetDelegatorInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task StakeInfoAsync(
        [CommandParameterCompletion(nameof(GetAllAddresses))]
        Address address = default,
        CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var actualAddress = address == default ? node.Address : address;
        var delegation = node.GetRequiredKeyedService<INodeDelegation>(INode.Key);
        var info = await delegation.GetStakeInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetDelegatorAddresses() => GetAddresses(IConsole.Tag, IClient.Tag);

    private string[] GetAllAddresses() => GetAddresses(IConsole.Tag, INode.Tag, IClient.Tag);
}
