using System.Net;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Clients.Serializations;

namespace LibplanetConsole.Consoles;

public interface IClient : IAddressable, IAsyncDisposable, IServiceProvider
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsRunning { get; }

    EndPoint EndPoint { get; }

    ClientInfo Info { get; }

    ClientOptions ClientOptions { get; }

    PrivateKey PrivateKey { get; }

    PublicKey PublicKey => PrivateKey.PublicKey;

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
