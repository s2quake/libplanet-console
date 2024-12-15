namespace LibplanetConsole.Client.Guild;

public interface IGuild
{
    Task<GuildInfo> CreateAsync(Address validatorAddress, CancellationToken cancellationToken);

    Task<Address> DeleteAsync(CancellationToken cancellationToken);

    Task LeaveAsync(CancellationToken cancellationToken);

    Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task<GuildInfo> GetGuildAsync(Address address, CancellationToken cancellationToken);
}
