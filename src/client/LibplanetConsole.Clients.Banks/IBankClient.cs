using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients.Banks;

internal interface IBankClient
{
    Task<BalanceInfo> MintAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        double amount, AppAddress targetAddress, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(AppAddress address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
