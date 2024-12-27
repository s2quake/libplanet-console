using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Client.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
[Category(nameof(Guild))]
internal sealed class GuildCommand(IClient client, IGuild guild) : CommandMethodBase
{
    [CommandMethod]
    public async Task CreateAsync(Address validatorAddress, CancellationToken cancellationToken)
    {
        await guild.CreateAsync(validatorAddress, cancellationToken);
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
    public async Task InfoAsync(string address = "", CancellationToken cancellationToken = default)
    {
        var memberAddress = address == string.Empty ? client.Address : new Address(address);
        var info = await guild.GetInfoAsync(memberAddress, cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
