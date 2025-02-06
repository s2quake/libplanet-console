using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
[Category(nameof(Guild))]
internal sealed class GuildCommand(IClient client, IGuild guild) : CommandMethodBase
{
    [CommandMethod]
    public async Task CreateAsync(Address nodeAddress, CancellationToken cancellationToken)
    {
        await guild.CreateAsync(nodeAddress, cancellationToken);
        var info = await guild.GetInfoAsync(client.Address, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }

    [CommandMethod]
    public async Task DeleteAsync(CancellationToken cancellationToken)
    {
        await guild.DeleteAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task JoinAsync(Address guildAddress, CancellationToken cancellationToken)
    {
        await guild.JoinAsync(guildAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task LeaveAsync(CancellationToken cancellationToken)
    {
        await guild.LeaveAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task BanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        await guild.BanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        await guild.UnbanAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(
        Address address = default, CancellationToken cancellationToken = default)
    {
        var actualAddress = address == default ? client.Address : address;
        var info = await guild.GetInfoAsync(actualAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
