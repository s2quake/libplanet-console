using System.Net;
using Libplanet.Action;
using Libplanet.Types.Tx;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.Executable;

public interface INode : IAddressable
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler? Stopped;

    bool IsRunning { get; }

    EndPoint EndPoint { get; }

    NodeInfo Info { get; }

    NodeOptions NodeOptions { get; }

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
