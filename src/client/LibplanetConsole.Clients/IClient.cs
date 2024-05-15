using Libplanet.Crypto;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Clients;

public interface IClient
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler<StopEventArgs>? Stopped;

    ClientInfo Info { get; }

    bool IsRunning { get; }

    Address Address { get; }

    Task StartAsync(ClientOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
