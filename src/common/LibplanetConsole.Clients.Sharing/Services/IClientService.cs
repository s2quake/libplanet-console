using JSSoft.Communication;
using LibplanetConsole.Clients.Serializations;

namespace LibplanetConsole.Clients.Services;

public interface IClientService
{
    [ServerMethod]
    Task<ClientInfo> StartAsync(
        ClientOptionsInfo clientOptionsInfo, CancellationToken cancellationToken);

    [ServerMethod]
    Task StopAsync(CancellationToken cancellationToken);

    [ServerMethod]
    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);
}
