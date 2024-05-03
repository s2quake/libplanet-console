using LibplanetConsole.ClientServices;
using LibplanetConsole.ClientServices.Serializations;

namespace LibplanetConsole.Executable;

public interface IClient : IIdentifier, IServiceProvider
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    bool IsRunning { get; }

    string Identifier { get; }

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task StartAsync(ClientOptions clientOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}
