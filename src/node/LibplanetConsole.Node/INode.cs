using Libplanet.Action;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

public interface INode : IVerifier, ISigner, IServiceProvider
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    NodeInfo Info { get; }

    bool IsRunning { get; }

    AppPublicKey PublicKey { get; }

    AppAddress Address => PublicKey.Address;

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<AppId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
