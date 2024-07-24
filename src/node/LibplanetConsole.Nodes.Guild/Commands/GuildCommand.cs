using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Guild;

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
        var guildAddress = await guildNode.GetGuildAsync(node.Address, default);
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
        var guildAddress = await guildNode.DeleteAsync(options, cancellationToken);
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RequestJoinAsync(
        AppAddress guildAddress, CancellationToken cancellationToken = default)
    {
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress,
        };
        var memberAddress = node.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress, memberAddress);
        await guildNode.RequestJoinAsync(options, cancellationToken);
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
        await guildNode.CancelJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task AcceptJoinAsync(
        AppAddress memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress);
        await guildNode.AcceptJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RejectJoinAsync(
        AppAddress memberAddress, CancellationToken cancellationToken = default)
    {
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress);
        await guildNode.RejectJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildNode.QuitAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    public async Task BanMemberAsync(
        AppAddress memberAddress, CancellationToken cancellationToken)
    {
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
        await guildNode.BanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    public async Task UnbanMemberAsync(
        AppAddress memberAddress, CancellationToken cancellationToken)
    {
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress,
        };
        var guildAddress = guildNode.Info.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
        await guildNode.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    public async Task ListMembersAsync(
        AppAddress guildAddress = default, CancellationToken cancellationToken = default)
    {
        var members = await guildNode.GetGuildMembersAsync(GetGuildAddress(), cancellationToken);
        var sb = new StringBuilder();
        for (var i = 0; i < members.Length; i++)
        {
            sb.AppendLine($"[{i:d2}] {members[i]}");
        }

        await Out.WriteLineAsync(sb.ToString());

        AppAddress GetGuildAddress()
            => guildAddress == default ? guildNode.Info.Address : guildAddress;
    }
}
