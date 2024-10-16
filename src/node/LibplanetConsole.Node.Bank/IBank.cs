using LibplanetConsole.Bank;

namespace LibplanetConsole.Node.Bank;

public interface IBank
{
    Task<BalanceInfo> MintAsync(decimal amount, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        Address targetAddress, decimal amount, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(decimal amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);

    Task<decimal> GetInitialSupplyAsync(CancellationToken cancellationToken);

    Task<decimal> GetSupplyAsync(CancellationToken cancellationToken);
}
