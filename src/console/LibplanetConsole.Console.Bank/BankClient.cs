using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Console.Services;

namespace LibplanetConsole.Console.Bank;

internal sealed class BankClient(IClient client) : ClientContentBase("bank-client"), IBank
{
    private IBankService Service => throw new NotImplementedException();

    async Task<BalanceInfo> IBank.MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        return await Service.MintAsync(mintOptions.Sign(client), cancellationToken);
    }

    async Task<BalanceInfo> IBank.TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        return await Service.TransferAsync(transferOptions.Sign(client), cancellationToken);
    }

    async Task<BalanceInfo> IBank.BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        return await Service.BurnAsync(burnOptions.Sign(client), cancellationToken);
    }

    Task<BalanceInfo> IBank.GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
        => Service.GetBalanceAsync(address, cancellationToken);

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
