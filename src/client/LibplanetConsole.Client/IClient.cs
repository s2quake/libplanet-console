using LibplanetConsole.Common;

namespace LibplanetConsole.Client;

public interface IClient : IVerifier
{
    event EventHandler? Started;

    event EventHandler? Stopped;

    ClientInfo Info { get; }

    bool IsRunning { get; }

    PublicKey PublicKey { get; }

    Address Address { get; }

    Uri NodeUrl { get; set; }

    Task StartAsync(CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
