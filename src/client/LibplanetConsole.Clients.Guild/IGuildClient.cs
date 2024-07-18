using LibplanetConsole.Guild;

namespace LibplanetConsole.Clients.Guild;

public interface IGuildClient
{
    Task CreateAsync(CreateGuildOptions options, CancellationToken cancellationToken);

    Task DeleteAsync(DeleteGuildOptions options, CancellationToken cancellationToken);

    Task RequestJoinAsync(RequestJoinOptions options, CancellationToken cancellationToken);

    Task CancelJoinAsync(CancelJoinOptions options, CancellationToken cancellationToken);

    Task AcceptJoinAsync(AcceptJoinOptions options, CancellationToken cancellationToken);

    Task RejectJoinAsync(RejectJoinOptions options, CancellationToken cancellationToken);

    Task QuitAsync(LeaveGuildOptions options, CancellationToken cancellationToken);

    Task BanMemberAsync(BanMemberOptions options, CancellationToken cancellationToken);

    Task UnbanMemberAsync(UnbanMemberOptions options, CancellationToken cancellationToken);
}
