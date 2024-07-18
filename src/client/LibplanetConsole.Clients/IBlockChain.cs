using Libplanet.Action;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public interface IBlockChain
{
    Task<AppId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);

    Task<long> GetNextNonceAsync(AppAddress address, CancellationToken cancellationToken);

    Task<AppHash> GetTipHashAsync(CancellationToken cancellationToken);

    Task<byte[]> GetStateByBlockHashAsync(
        AppHash blockHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<byte[]> GetStateByStateRootHashAsync(
        AppHash stateRootHash,
        AppAddress accountAddress,
        AppAddress address,
        CancellationToken cancellationToken);

    Task<AppHash> GetBlockHashAsync(long height, CancellationToken cancellationToken);
}
