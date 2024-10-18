using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Guild;
using LibplanetConsole.Node;

namespace LibplanetConsole.Node.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
internal sealed class GuildCommand(INode node, IGuild guild) : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Create new guild.")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guild.CreateAsync(options, cancellationToken);
        var guildAddress = await guild.GetGuildAsync(node.Address, default);
        var message = GuildEventMessage.CreatedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod(Aliases = ["rm"])]
    [CommandSummary("Delete the guild.")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var options = new DeleteGuildOptions
        {
        };
        var guildAddress = await guild.DeleteAsync(options, cancellationToken);
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RequestJoinAsync(
        Address guildAddress, CancellationToken cancellationToken = default)
    {
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress,
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
        await guild.RequestJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var options = new CancelJoinOptions
        {
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.CanceledJoinMessage(memberAddress);
        await guild.CancelJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task AcceptJoinAsync(
        Address memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guild.Info.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
        await guild.AcceptJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RejectJoinAsync(
        Address memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guild.Info.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
        await guild.RejectJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        var guildAddress = guild.Info.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guild.QuitAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    public async Task BanMemberAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guild.Info.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
        await guild.BanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    public async Task UnbanMemberAsync(
        Address memberAddress, CancellationToken cancellationToken)
    {
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guild.Info.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
        await guild.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    public async Task ListMembersAsync(
        Address guildAddress = default, CancellationToken cancellationToken = default)
    {
        var members = await guild.GetGuildMembersAsync(GetGuildAddress(), cancellationToken);
        var sb = new StringBuilder();
        for (var i = 0; i < members.Length; i++)
        {
            sb.AppendLine($"[{i:d2}] {members[i]}");
        }

        await Out.WriteLineAsync(sb.ToString());

        Address GetGuildAddress()
            => guildAddress == default ? guild.Info.Address : guildAddress;
    }
}
