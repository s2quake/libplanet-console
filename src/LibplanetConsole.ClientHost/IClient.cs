using Libplanet.Crypto;
using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.NodeServices;

namespace LibplanetConsole.ClientHost;

public interface IClient
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler<StopEventArgs>? Stopped;

    ClientInfo Info { get; }

    bool IsRunning { get; }

    PrivateKey PrivateKey { get; }

    PublicKey PublicKey => PrivateKey.PublicKey;

    Address Address => PrivateKey.Address;

    Task StartAsync(ClientOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
