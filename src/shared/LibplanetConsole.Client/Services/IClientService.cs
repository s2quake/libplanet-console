namespace LibplanetConsole.Client.Services;

public interface IClientService
{
    Task<ClientInfo> StartAsync(string nodeEndPoint, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<ClientInfo> GetInfoAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(
        TransactionOptions transactionOptions, CancellationToken cancellationToken);
}
