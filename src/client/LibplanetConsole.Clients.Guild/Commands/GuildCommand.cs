using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Clients.Guild.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the guild service.")]
[method: ImportingConstructor]
internal sealed class GuildCommand(IClient client, IGuildClient guildClient) : CommandMethodBase
{
    [CommandMethod]
    [CommandSummary("Create new guild.")]
    public async Task NewAsync(CancellationToken cancellationToken = default)
    {
        var options = new CreateGuildOptions
        {
            Name = "Guild",
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.CreatedMessage(guildAddress);
        await guildClient.CreateAsync(options, cancellationToken);
        await Out.WriteLineAsJsonAsync(message);
    }

    [CommandMethod(Aliases = ["rm"])]
    [CommandSummary("Delete the guild.")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var options = new DeleteGuildOptions
        {
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.DeletedMessage(guildAddress);
        await guildClient.DeleteAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RequestJoinAsync(
        string guildAddress, CancellationToken cancellationToken = default)
    {
        var guildAddress1 = AppAddress.Parse(guildAddress);
        var options = new RequestJoinOptions
        {
            GuildAddress = guildAddress1,
        };
        var memberAddress = client.Address;
        var message = GuildEventMessage.RequestedJoinMessage(guildAddress1, memberAddress);
        await guildClient.RequestJoinAsync(options, cancellationToken);
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
        var memberAddress = client.Address;
        var message = GuildEventMessage.CancelledJoinMessage(memberAddress);
        await guildClient.CancelJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task AcceptJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new AcceptJoinOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.AcceptedJoinMessage(guildAddress, memberAddress1);
        await guildClient.AcceptJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Request to join the guild.")]
    public async Task RejectJoinAsync(
        string memberAddress, CancellationToken cancellationToken = default)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new RejectJoinOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.RejectedJoinMessage(guildAddress, memberAddress1);
        await guildClient.RejectJoinAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.LeftMessage(guildAddress, guildAddress);
        await guildClient.QuitAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Ban the member.")]
    public async Task BanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new BanMemberOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.BannedMessage(guildAddress, memberAddress1);
        await guildClient.BanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }

    [CommandMethod]
    [CommandSummary("Unban the member.")]
    public async Task UnbanMemberAsync(
        string memberAddress, CancellationToken cancellationToken)
    {
        var memberAddress1 = AppAddress.Parse(memberAddress);
        var options = new UnbanMemberOptions
        {
            MemberAddress = memberAddress1,
        };
        var guildAddress = client.Address;
        var message = GuildEventMessage.UnbannedMessage(guildAddress, memberAddress1);
        await guildClient.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync(message);
    }
}
