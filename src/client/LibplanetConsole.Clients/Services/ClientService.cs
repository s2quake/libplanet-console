using System.ComponentModel.Composition;
using LibplanetConsole.Clients.Serializations;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Services;

[Export(typeof(ILocalService))]
[Export(typeof(IClientService))]
internal sealed class ClientService : LocalService<IClientService, IClientCallback>, IClientService
{
    private readonly Client _client;

    [ImportingConstructor]
    public ClientService(Client client)
    {
        _client = client;
        _client.Started += (s, e) => Callback.OnStarted(_client.Info);
        _client.Stopped += (s, e) => Callback.OnStopped();
    }

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
