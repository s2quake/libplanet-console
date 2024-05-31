using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;

namespace LibplanetConsole.Banks.Services;

public interface IBankService
{
    Task<BalanceInfo> MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
