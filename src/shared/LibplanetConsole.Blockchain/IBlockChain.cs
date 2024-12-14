using System.Security.Cryptography;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public interface IBlockChain
{
    event EventHandler<BlockEventArgs>? BlockAppended;

    event EventHandler? Started;

    event EventHandler? Stopped;

    bool IsRunning { get; }

    BlockInfo Tip { get; }

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        long height,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        BlockHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<IValue> GetStateAsync(
        HashDigest<SHA256> stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<BlockHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<T> GetActionAsync<T>(TxId txId, int actionIndex, CancellationToken cancellationToken)
        where T : IAction;

    Task<AddressInfo[]> GetAddressesAsync(CancellationToken cancellationToken);

#if LIBPLANET_NODE
    Libplanet.Action.State.IWorldState GetWorldState();

    Libplanet.Action.State.IWorldState GetWorldState(BlockHash offset);

    Libplanet.Action.State.IWorldState GetWorldState(long height);
#endif // LIBPLANET_NODE
}
