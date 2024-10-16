using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Client.Bank.Services;

internal sealed class BankService(IClient client, IBank bank)
    : LocalService<IBankService>, IBankService
{
    public Task<BalanceInfo> MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        mintOptions.Verify(client);
        return bank.MintAsync(mintOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        transferOptions.Verify(client);
        return bank.TransferAsync(
            amount: transferOptions.Amount,
            targetAddress: transferOptions.TargetAddress,
            cancellationToken: cancellationToken);
    }

    public Task<BalanceInfo> BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        burnOptions.Verify(client);
        return bank.BurnAsync(burnOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
        => bank.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => bank.GetPoolAsync(cancellationToken);
}
