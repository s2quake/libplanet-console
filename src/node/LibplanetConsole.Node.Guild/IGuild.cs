namespace LibplanetConsole.Node.Guild;

public interface IGuild
{
    Task<GuildInfo> CreateAsync(CancellationToken cancellationToken);

    Task<Address> DeleteAsync(CancellationToken cancellationToken);

    Task JoinAsync(Address guildAddress, CancellationToken cancellationToken);

    Task LeaveAsync(CancellationToken cancellationToken);

    Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task ClaimAsync(CancellationToken cancellationToken);

    Task<GuildInfo> GetGuildAsync(CancellationToken cancellationToken);
}
