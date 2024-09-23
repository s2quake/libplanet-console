using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Services;

public interface IClientService
{
    Task<ClientInfo> StartAsync(string nodeEndPoint, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<AppId> SendTransactionAsync(
        TransactionOptions transactionOptions, CancellationToken cancellationToken);
}
