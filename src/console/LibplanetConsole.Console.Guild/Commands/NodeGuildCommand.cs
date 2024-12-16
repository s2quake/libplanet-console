using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Console.Commands;
using LibplanetConsole.Console.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Guild.Commands;

internal sealed partial class NodeGuildCommand(NodeCommand nodeCommand, INodeCollection nodes)
    : CommandMethodBase(nodeCommand, "guild")
{
    [CommandPropertyRequired(DefaultValue = "")]
    [CommandSummary("The address of the node. If not specified, the current node is used.")]
    [CommandPropertyCompletion(nameof(GetNodeAddresses))]
    public static string Address { get; set; } = string.Empty;

    [CommandMethod]
    public async Task CreateAsync(
        CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.CreateAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(
        string memberAddress = "", CancellationToken cancellationToken = default)
    {
        var node = nodes.GetNodeOrCurrent(Address);
        var guild = node.GetRequiredKeyedService<INodeGuild>(INode.Key);
        var info = await guild.GetInfoAsync(node.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    private string[] GetNodeAddresses() => [.. nodes.Select(node => node.Address.ToString())];
}
