// using LibplanetConsole.Common;

// namespace LibplanetConsole.Guild.Services;

// public interface IGuildService
// {
//     Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken);

//     Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

//     Task<AppAddress> DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

//     Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken);

//     Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken);

//     Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken);

//     Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken);

//     Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

//     Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

//     Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);

//     Task<AppAddress> GetGuildAsync(
//         long height, AppAddress address, CancellationToken cancellationToken);

//     Task<AppAddress[]> GetGuildMembersAsync(
//         long height, AppAddress guildAddress, CancellationToken cancellationToken);
// }
