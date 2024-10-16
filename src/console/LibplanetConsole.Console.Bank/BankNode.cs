using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Console.Services;

namespace LibplanetConsole.Console.Bank;

internal sealed class BankNode(INode node) : NodeContentBase("bank-node"), IBank
{
    private IBankService Service => throw new NotImplementedException();

    Task<BalanceInfo> IBank.MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        return Service.MintAsync(mintOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBank.TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        return Service.TransferAsync(transferOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBank.BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        return Service.BurnAsync(burnOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBank.GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
    {
        return Service.GetBalanceAsync(address, cancellationToken: cancellationToken);
    }

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
