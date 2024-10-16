using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Guild;
using LibplanetConsole.Guild.Services;

namespace LibplanetConsole.Client.Guild.Services;

internal sealed class GuildClientService(IClient client, Guild guildClient)
    : LocalService<IGuildClientService>, IGuildClientService
{
    public Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken)
        => guildClient.CreateAsync(options.Verify(client), cancellationToken);

    public Task<Address> DeleteAsync(
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

    public Task<Address> GetGuildAsync(
        long height, Address address, CancellationToken cancellationToken)
        => guildClient.GetGuildAsync(height, address, cancellationToken);

    public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
        => Task.Run(() => guildClient.Info, cancellationToken);

    public Task<Address[]> GetGuildMembersAsync(
        long height, Address guildAddress, CancellationToken cancellationToken)
        => guildClient.GetGuildMembersAsync(height, guildAddress, cancellationToken);
}
