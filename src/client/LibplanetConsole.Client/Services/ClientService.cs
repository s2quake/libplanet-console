using LibplanetConsole.Common;
using LibplanetConsole.Common.Actions;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Services;

internal sealed class ClientService : LocalService<IClientService, IClientCallback>, IClientService
{
    private readonly Client _client;

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
        string nodeEndPoint, CancellationToken cancellationToken)
    {
        _client.NodeEndPoint = EndPointUtility.Parse(nodeEndPoint);
        await _client.StartAsync(cancellationToken);
        return _client.Info;
    }

    public Task StopAsync(CancellationToken cancellationToken)
        => _client.StopAsync(cancellationToken);

    public async Task<TxId> SendTransactionAsync(
        TransactionOptions transactionOptions, CancellationToken cancellationToken)
    {
        if (transactionOptions.TryVerify(_client) == true)
        {
            var action = new StringAction
            {
                Value = transactionOptions.Text,
            };
            return await _client.SendTransactionAsync([action], cancellationToken);
        }

        throw new InvalidOperationException("The signature is invalid.");
    }
}
