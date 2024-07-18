using System.ComponentModel.Composition;
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
    {
        options.Verify(client);
        return guildClient.CreateAsync(options, cancellationToken);
    }

    public Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.DeleteAsync(options, cancellationToken);
    }

    public Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.RequestJoinAsync(options, cancellationToken);
    }

    public Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.CancelJoinAsync(options, cancellationToken);
    }

    public Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.AcceptJoinAsync(options, cancellationToken);
    }

    public Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.RejectJoinAsync(options, cancellationToken);
    }

    public Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.QuitAsync(options, cancellationToken);
    }

    public Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.BanMemberAsync(options, cancellationToken);
    }

    public Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken)
    {
        options.Verify(client);
        return guildClient.UnbanMemberAsync(options, cancellationToken);
    }

    public Task<GuildInfo> StartAsync(CancellationToken cancellationToken)
    {
        return guildClient.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return guildClient.StopAsync(cancellationToken);
    }

    public Task<GuildInfo> GetGuildInfoAsync(CancellationToken cancellationToken)
    {
        return Task.Run(() => guildClient.Info, cancellationToken);
    }
}
