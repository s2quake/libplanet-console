using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;

namespace LibplanetConsole.Nodes.Banks;

public interface IBankNode
{
    Task<BalanceInfo> MintAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        Address targetAddress, double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(double amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
