using System.Net;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.Nodes;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Executable;

public interface INode : IAddressable, IAsyncDisposable, IServiceProvider
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsRunning { get; }

    EndPoint EndPoint { get; }

    NodeInfo Info { get; }

    NodeOptions NodeOptions { get; }

    PrivateKey PrivateKey { get; }

    PublicKey PublicKey => PrivateKey.PublicKey;

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
