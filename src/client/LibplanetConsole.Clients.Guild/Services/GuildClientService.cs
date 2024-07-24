using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Clients.Guild.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class GuildClientService(IClient client, GuildClient guildClient)
    : LocalService<IGuildClientService>, IGuildClientService
{
    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
        => guildClient.CreateAsync(options.Verify(client), cancellationToken);

    public Task<AppAddress> DeleteAsync(
        DeleteGuildOptions options, CancellationToken cancellationToken)
        => guildClient.DeleteAsync(options.Verify(client), cancellationToken);

    public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
        => guildClient.RequestJoinAsync(options.Verify(client), cancellationToken);

    public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
        => guildClient.CancelJoinAsync(options.Verify(client), cancellationToken);

    public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
        => guildClient.AcceptJoinAsync(options.Verify(client), cancellationToken);

    public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
        => guildClient.RejectJoinAsync(options.Verify(client), cancellationToken);

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
        => guildClient.QuitAsync(options.Verify(client), cancellationToken);

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
        => guildClient.BanMemberAsync(options.Verify(client), cancellationToken);

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
        => guildClient.UnbanMemberAsync(options.Verify(client), cancellationToken);

    public Task<AppAddress> GetGuildAsync(
        long height, AppAddress address, CancellationToken cancellationToken)
        => guildClient.GetGuildAsync(height, address, cancellationToken);

    public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
        => Task.Run(() => guildClient.Info, cancellationToken);

    public Task<AppAddress[]> GetGuildMembersAsync(
        long height, AppAddress guildAddress, CancellationToken cancellationToken)
        => guildClient.GetGuildMembersAsync(height, guildAddress, cancellationToken);
}
