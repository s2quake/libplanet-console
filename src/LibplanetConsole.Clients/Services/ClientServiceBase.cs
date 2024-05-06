using JSSoft.Communication;
using LibplanetConsole.Clients.Serializations;

namespace LibplanetConsole.Clients.Services;

public abstract class ClientServiceBase(IClient client)
        : ServerService<IClientService, IClientCallback>, IClientService
{
    private readonly IClient _client = client;

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

    protected override IClientService CreateServer(IPeer peer)
    {
        return base.CreateServer(peer);
    }

    protected override void DestroyServer(IPeer peer, IClientService server)
    {
        base.DestroyServer(peer, server);
    }
}
