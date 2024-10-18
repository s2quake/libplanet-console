// using LibplanetConsole.Common;

// namespace LibplanetConsole.Guild.Services;

// public interface IGuildClientService
// {
//     Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken);

//     Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

//     Task<Address> DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

//     Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken);

//     Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken);

//     Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken);

//     Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken);

//     Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

//     Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

//     Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);

//     Task<Address> GetGuildAsync(
//         long height, Address address, CancellationToken cancellationToken);

//     Task<Address[]> GetGuildMembersAsync(
//         long height, Address guildAddress, CancellationToken cancellationToken);
// }
