using LibplanetConsole.Clients.Serializations;

namespace LibplanetConsole.Clients.Services;

public interface IClientService
{
    Task<ClientInfo> StartAsync(
        ClientOptionsInfo clientOptionsInfo, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);
}
