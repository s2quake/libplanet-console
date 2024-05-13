using System.ComponentModel.Composition;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Services;

[Export(typeof(ILocalService))]
[Export(typeof(IClientService))]
[method: ImportingConstructor]
internal sealed class ClientService(Client client)
    : LocalService<IClientService, IClientCallback>, IClientService
{
    private readonly Client _client = client;

    public async Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        return _client.Info;
    }

    public async Task<ClientInfo> StartAsync(
        ClientOptionsInfo clientOptionsInfo, CancellationToken cancellationToken)
    {
        await _client.StartAsync(clientOptionsInfo, cancellationToken);
        return _client.Info;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _client.StopAsync(cancellationToken);
}
