using System.Text;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Client.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
internal sealed class GuildCommand(IClient client, IGuild guildClient) : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Create new guild.")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        await guildClient.CreateAsync(options, cancellationToken);
        var guildAddress = await guildClient.GetGuildAsync(client.Address, cancellationToken);
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
        var guildAddress = await guildClient.DeleteAsync(options, cancellationToken);
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
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildClient.QuitAsync(options, cancellationToken);
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
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress);
        await guildClient.BanMemberAsync(options, cancellationToken);
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
        var guildAddress = guildClient.Info.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress);
        await guildClient.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("List the member.")]
    public async Task ListMembersAsync(
        Address guildAddress = default, CancellationToken cancellationToken = default)
    {
        var members = await guildClient.GetGuildMembersAsync(GetGuildAddress(), cancellationToken);
        var sb = new StringBuilder();
        for (var i = 0; i < members.Length; i++)
        {
            sb.AppendLine($"[{i:d2}] {members[i]}");
        }

        await Out.WriteLineAsync(sb.ToString());

        Address GetGuildAddress()
            => guildAddress == default ? guildClient.Info.Address : guildAddress;
    }
}
