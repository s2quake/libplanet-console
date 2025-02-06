using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;

namespace LibplanetConsole.Console.Guild.Commands;

internal sealed partial class NodeGuildCommand(
    IServiceProvider serviceProvider,
    NodeCommand nodeCommand)
    : NodeCommandMethodBase(serviceProvider, nodeCommand, "guild")
{
    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task CreateAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address validatorAddress,
        CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.CreateAsync(validatorAddress, cancellationToken);
        var info = await guild.GetInfoAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var node = CurrentNode;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task InfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var node = CurrentNode;
        var actualAddress = address == default ? node.Address : address;
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        var info = await guild.GetInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
