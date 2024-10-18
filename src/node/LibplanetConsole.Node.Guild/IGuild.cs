using LibplanetConsole.Common;
using LibplanetConsole.Guild;

namespace LibplanetConsole.Node.Guild;

public interface IGuild
{
    GuildInfo Info { get; }

    Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

    Task<Address> DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

    Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

    Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

    Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);

    Task<Address> GetGuildAsync(Address address, CancellationToken cancellationToken)
        => GetGuildAsync(long.MaxValue, address, cancellationToken);

    Task<Address> GetGuildAsync(
        long height, Address address, CancellationToken cancellationToken);

    Task<Address[]> GetGuildMembersAsync(
        Address guildAddress, CancellationToken cancellationToken)
        => GetGuildMembersAsync(long.MaxValue, guildAddress, cancellationToken);

    Task<Address[]> GetGuildMembersAsync(
        long height, Address guildAddress, CancellationToken cancellationToken);
}
