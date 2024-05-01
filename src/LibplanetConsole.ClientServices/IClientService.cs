using JSSoft.Communication;
using LibplanetConsole.ClientServices.Serializations;

namespace LibplanetConsole.ClientServices;

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
