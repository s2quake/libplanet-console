using LibplanetConsole.Bank;

namespace LibplanetConsole.Client.Bank;

internal interface IBank
{
    Task<BalanceInfo> MintAsync(decimal amount, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        decimal amount, Address targetAddress, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(decimal amount, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);

    Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken);
}
