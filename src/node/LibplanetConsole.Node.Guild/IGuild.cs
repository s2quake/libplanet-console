namespace LibplanetConsole.Node.Guild;

public interface IGuild
{
    Task CreateAsync(Address validatorAddress, CancellationToken cancellationToken);

    Task DeleteAsync(CancellationToken cancellationToken);

    Task JoinAsync(Address guildAddress, CancellationToken cancellationToken);

    Task LeaveAsync(CancellationToken cancellationToken);

    Task MoveAsync(Address guildAddress, CancellationToken cancellationToken);

    Task BanAsync(Address memberAddress, CancellationToken cancellationToken);

    Task UnbanAsync(Address memberAddress, CancellationToken cancellationToken);

    Task ClaimAsync(CancellationToken cancellationToken);

    Task<GuildInfo> GetInfoAsync(Address memberAddress, CancellationToken cancellationToken);
}
