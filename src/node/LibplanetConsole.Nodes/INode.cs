using Libplanet.Action;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public interface INode : IVerifier, ISigner, IServiceProvider
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    NodeInfo Info { get; }

    NodeOptions NodeOptions { get; }

    bool IsRunning { get; }

    AppPublicKey PublicKey { get; }

    AppAddress Address => PublicKey.Address;

    AppPeer BlocksyncSeedPeer { get; }

    AppPeer ConsensusSeedPeer { get; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<AppId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
