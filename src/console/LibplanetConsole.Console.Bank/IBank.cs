using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;

namespace LibplanetConsole.Console.Bank;

internal interface IBank
{
    Task<BalanceInfo> MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken);

    Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken);
}
