using System.Net;
using Libplanet.Crypto;
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

    PublicKey PublicKey { get; }

    byte[] Sign(object obj);

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
