namespace LibplanetConsole.Bank.Services;

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
