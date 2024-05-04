using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Serializations;

namespace LibplanetConsole.Executable;

public interface IClient : IIdentifier, IAsyncDisposable
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    event EventHandler? Disposed;

    bool IsRunning { get; }

    string Identifier { get; }

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
