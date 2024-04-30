using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Tx;
using LibplanetConsole.NodeServices;
using LibplanetConsole.NodeServices.Serializations;

namespace LibplanetConsole.NodeHost;

public interface INode
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    NodeInfo Info { get; }

    NodeOptions NodeOptions { get; }

    bool IsRunning { get; }

    BlockChain BlockChain { get; }

    PrivateKey PrivateKey { get; }

    PublicKey PublicKey => PrivateKey.PublicKey;

    Address Address => PrivateKey.Address;

    Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);
}
