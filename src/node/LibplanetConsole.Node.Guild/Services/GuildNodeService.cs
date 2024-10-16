// // using LibplanetConsole.Common;
// using LibplanetConsole.Common.Services;
// using LibplanetConsole.Guild;
// using LibplanetConsole.Guild.Services;

// namespace LibplanetConsole.Node.Guild.Services;

// [Export(typeof(ILocalService))]
// internal sealed class GuildNodeService(INode node, GuildNode guild)
//         : LocalService<IGuildService>, IGuildService
// {
//     public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
//         => guild.CreateAsync(options.Verify(node), cancellationToken);

//     public Task<AppAddress> DeleteAsync(
//         DeleteGuildOptions options, CancellationToken cancellationToken)
//         => guild.DeleteAsync(options.Verify(node), cancellationToken);

//     public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
//         => guild.QuitAsync(options.Verify(node), cancellationToken);

//     public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
//         => guild.RequestJoinAsync(options.Verify(node), cancellationToken);

//     public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
//         => guild.CancelJoinAsync(options.Verify(node), cancellationToken);

//     public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
//         => guild.AcceptJoinAsync(options.Verify(node), cancellationToken);

//     public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
//         => guild.RejectJoinAsync(options.Verify(node), cancellationToken);

//     public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
//         => guild.BanMemberAsync(options.Verify(node), cancellationToken);

//     public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
//         => guild.UnbanMemberAsync(options.Verify(node), cancellationToken);

//     public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
//         => Task.Run(() => guild.Info, cancellationToken);

//     public Task<AppAddress> GetGuildAsync(
//         long height, AppAddress address, CancellationToken cancellationToken)
//         => guild.GetGuildAsync(height, address, cancellationToken);

//     public Task<AppAddress[]> GetGuildMembersAsync(
//         long height, AppAddress guildAddress, CancellationToken cancellationToken)
//         => guild.GetGuildMembersAsync(height, guildAddress, cancellationToken);
// }
