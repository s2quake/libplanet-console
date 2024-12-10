namespace LibplanetConsole.Console.Guild;

public interface IConsoleGuild
{
    GuildInfo Info { get; }

    Task CreateAsync(string name, CancellationToken cancellationToken);

    Task<Address> DeleteAsync(CancellationToken cancellationToken);

    Task QuitAsync(CancellationToken cancellationToken);

    Task BanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

    Task UnbanMemberAsync(Address memberAddress, CancellationToken cancellationToken);

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
