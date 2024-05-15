using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Net;
using Libplanet.Types.Tx;
using LibplanetConsole.Nodes.Serializations;

namespace LibplanetConsole.Nodes;

public interface INode
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler? Stopped;

    NodeInfo Info { get; }

    NodeOptions NodeOptions { get; }

    bool IsRunning { get; }

    BlockChain BlockChain { get; }

    PublicKey PublicKey { get; }

    Address Address => PublicKey.Address;

    BoundPeer BlocksyncSeedPeer { get; }

    BoundPeer ConsensusSeedPeer { get; }

    Task StartAsync(NodeOptions nodeOptions, CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);

    Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);
}
