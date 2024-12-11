namespace LibplanetConsole.Console.Guild;

public interface IGuild
{
    Task CreateAsync(Address validatorAddress, CancellationToken cancellationToken);

    Task DeleteAsync(CancellationToken cancellationToken);

    Task JoinAsync(Address guildAddress, CancellationToken cancellationToken);

    Task LeaveAsync(CancellationToken cancellationToken);

    Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task ClaimAsync(CancellationToken cancellationToken);

    Task<GuildInfo> GetGuildAsync(CancellationToken cancellationToken);
}
