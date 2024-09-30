using Libplanet.Crypto;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Services;

public interface IBlockChainService
{
    Task<AppId> SendTransactionAsync(byte[] transaction, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(Address address, CancellationToken cancellationToken);

    Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<byte[]> GetStateAsync(
        AppHash blockHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        Address accountAddress,
        Address address,
        CancellationToken cancellationToken);

    Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);

    Task<byte[]> GetActionAsync(AppId txId, int actionIndex, CancellationToken cancellationToken);
}
