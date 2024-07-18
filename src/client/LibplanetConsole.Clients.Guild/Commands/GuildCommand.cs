using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Clients.Guild.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Provides commands for the guild service.")]
[method: ImportingConstructor]
internal sealed class GuildCommand(IClient client, IGuildClient guildClient)
    : CommandMethodBase
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
        await Out.WriteLineAsync($"Guild created.: {client.Address}");
    }

    [CommandMethod(Aliases = ["rm"])]
    [CommandSummary("Delete the guild.")]
    public async Task DeleteAsync(CancellationToken cancellationToken = default)
    {
        var options = new DeleteGuildOptions
        {
        };
        await guildClient.DeleteAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Guild deleted.: {client.Address}");
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
        await guildClient.RequestJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Cancel the request to join the guild.")]
    public async Task CancelJoinAsync(
        CancellationToken cancellationToken = default)
    {
        var options = new CancelJoinOptions
        {
        };
        await guildClient.CancelJoinAsync(options, cancellationToken);
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
        await guildClient.AcceptJoinAsync(options, cancellationToken);
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
        await guildClient.RejectJoinAsync(options, cancellationToken);
    }

    [CommandMethod]
    [CommandSummary("Leave the guild.")]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        var options = new LeaveGuildOptions
        {
        };
        await guildClient.QuitAsync(options, cancellationToken);
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
        await guildClient.BanMemberAsync(options, cancellationToken);
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
        await guildClient.UnbanMemberAsync(options, cancellationToken);
        await Out.WriteLineAsync($"Unbanned the member.: {memberAddress}");
    }
}
