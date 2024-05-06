using Libplanet.Crypto;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public interface IClient
{
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
