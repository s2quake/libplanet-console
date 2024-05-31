using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;

namespace LibplanetConsole.Consoles.Banks;

internal interface IBankContent
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
