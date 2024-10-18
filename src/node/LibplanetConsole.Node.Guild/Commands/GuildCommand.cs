using System.ComponentModel;
using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Node.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
[Category(nameof(Guild))]
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
