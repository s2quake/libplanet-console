using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;

namespace LibplanetConsole.Banks;

internal interface IBankClient
{
    Task<BalanceInfo> MintAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
