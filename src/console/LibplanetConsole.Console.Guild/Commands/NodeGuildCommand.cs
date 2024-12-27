using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Guild.Commands;

internal sealed partial class NodeGuildCommand(NodeCommand nodeCommand, INodeCollection nodes)
    : CommandMethodBase(nodeCommand, "guild")
{
    [CommandProperty(InitValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string NodeAddress { get; set; } = string.Empty;

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task CreateAsync(
        [CommandParameterCompletion(nameof(GetNodeAddresses))]
        Address validatorAddress,
        CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.CreateAsync(validatorAddress, cancellationToken);
        var info = await guild.GetInfoAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    [CommandMethodProperty(nameof(NodeAddress))]
    public async Task InfoAsync(
        string memberAddress = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(NodeAddress);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        var info = await guild.GetInfoAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];
}
