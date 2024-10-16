using LibplanetConsole.Common;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Consoles.Guild;

public interface IGuildNodeContent
{
    GuildInfo Info { get; }

    Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

    Task<AppAddress> DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

    Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken);

    Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken);

    Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken);

    Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken);

    Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

    Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

    Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);

    Task<AppAddress> GetGuildAsync(AppAddress address, CancellationToken cancellationToken)
        => GetGuildAsync(long.MaxValue, address, cancellationToken);

    Task<AppAddress> GetGuildAsync(
        long height, AppAddress address, CancellationToken cancellationToken);

    Task<AppAddress[]> GetGuildMembersAsync(
        AppAddress guildAddress, CancellationToken cancellationToken)
        => GetGuildMembersAsync(long.MaxValue, guildAddress, cancellationToken);

    Task<AppAddress[]> GetGuildMembersAsync(
        long height, AppAddress guildAddress, CancellationToken cancellationToken);
}
