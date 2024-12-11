using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Guild.Commands;

[CommandSummary("Provides commands for the guild service.")]
[Category(nameof(Guild))]
internal sealed class GuildCommand(IGuild guild) : CommandMethodBase
{
    [CommandMethod]
    public async Task CreateAsync(CancellationToken cancellationToken)
    {
        await guild.CreateAsync(cancellationToken);
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
    public async Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        await guild.BanMemberAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken)
    {
        await guild.UnbanMemberAsync(memberAddress, cancellationToken);
    }

    [CommandMethod]
    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        await guild.ClaimAsync(cancellationToken);
    }

    [CommandMethod]
    public async Task InfoAsync(CancellationToken cancellationToken)
    {
        var info = await guild.GetGuildAsync(cancellationToken);
        await Out.WriteLineAsJsonAsync(info, cancellationToken);
    }
}
