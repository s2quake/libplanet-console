using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;

namespace LibplanetConsole.Console.Bank;

internal sealed class BankNode(INode node)
    : INodeContent, IBank, INodeContentService
{
    private readonly RemoteService<IBankService> _bankService = new();

    IRemoteService INodeContentService.RemoteService => _bankService;

    INode INodeContent.Node => node;

    string INodeContent.Name => "Bank";

    private IBankService Service => _bankService.Service;

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

    Task<PoolInfo> IBank.GetPoolAsync(CancellationToken cancellationToken)
    {
        return Service.GetPoolAsync(cancellationToken);
    }
}
