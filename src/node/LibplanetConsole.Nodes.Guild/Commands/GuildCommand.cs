using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Guild;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LibplanetConsole.Nodes.Guild.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the guild service.")]
[method: ImportingConstructor]
internal sealed class GuildCommand(INode node, IGuildNode guildNode) : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Create new guild.")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guildNode.CreateAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Guild created.: {node.Address}");
    }

    [CommandMethod(Aliases = ["rm"])]
    [CommandSummary("Delete the guild.")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var options = new DeleteGuildOptions
        {
        };
        await guildNode.DeleteAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Guild deleted.: {node.Address}");
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RequestJoinAsync(
        string guildAddress, CancellationToken cancellationToken = default)
    {
        var options = new RequestJoinOptions
        {
            GuildAddress = AppAddress.Parse(guildAddress),
        };
        await guildNode.RequestJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var options = new CancelJoinOptions
        {
        };
        await guildNode.CancelJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task AcceptJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new AcceptJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.AcceptJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RejectJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new RejectJoinOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.RejectJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        await guildNode.QuitAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Left the guild.");
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    public async Task BanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var options = new BanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.BanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Banned the member.: {memberAddress}");
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    public async Task UnbanMemberAsync(string memberAddress, CancellationToken cancellationToken)
    {
        var options = new UnbanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Unbanned the member.: {memberAddress}");
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    public async Task ListMembersAsync(string memberAddress, CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<IBlockChain>();
        var options = new UnbanMemberOptions
        {
            MemberAddress = AppAddress.Parse(memberAddress),
        };
        await guildNode.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Unbanned the member.: {memberAddress}");
    }
}
