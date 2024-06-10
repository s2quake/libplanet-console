using System.ComponentModel.Composition;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;

namespace LibplanetConsole.Consoles.Banks;

[Export(typeof(INodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(IBankContent))]
[method: ImportingConstructor]
internal sealed class BankNodeContent(INode node)
    : INodeContent, IBankContent, INodeContentService
{
    private readonly RemoteService<IBankService> _bankService = new();

    IRemoteService INodeContentService.RemoteService => _bankService;

    INode INodeContent.Node => node;

    string INodeContent.Name => "Bank";

    private IBankService Service => _bankService.Service;

    Task<BalanceInfo> IBankContent.MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        return Service.MintAsync(mintOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBankContent.TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        return Service.TransferAsync(transferOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBankContent.BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        return Service.BurnAsync(burnOptions.Sign(node), cancellationToken);
    }

    Task<BalanceInfo> IBankContent.GetBalanceAsync(
        AppAddress address, CancellationToken cancellationToken)
    {
        return Service.GetBalanceAsync(address, cancellationToken: cancellationToken);
    }

    Task<PoolInfo> IBankContent.GetPoolAsync(CancellationToken cancellationToken)
    {
        return Service.GetPoolAsync(cancellationToken);
    }
}
