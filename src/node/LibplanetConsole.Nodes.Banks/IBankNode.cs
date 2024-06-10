using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Banks;

public interface IBankNode
{
    Task<BalanceInfo> MintAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        AppAddress targetAddress, double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(AppAddress address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
