using System.Net;
using Libplanet.Action;
using Libplanet.Types.Tx;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.Executable;

public interface INode : IIdentifier, IServiceProvider
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    bool IsRunning { get; }

    string Identifier { get; }

    EndPoint EndPoint { get; }

    Task<NodeInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> AddTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
