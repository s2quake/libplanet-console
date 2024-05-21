using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;
using LibplanetConsole.Consoles.Services;

namespace LibplanetConsole.Banks;

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

    private IBankService Service => _bankService.Service;

    async Task<BalanceInfo> IBankContent.MintAsync(
        double amount, CancellationToken cancellationToken)
    {
        var signature = node.Sign(amount);
        return await Service.MintAsync(signature, amount, cancellationToken);
    }

    Task<BalanceInfo> IBankContent.GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
    {
        return Service.GetBalanceAsync(address, cancellationToken: cancellationToken);
    }

    Task<PoolInfo> IBankContent.GetPoolAsync(CancellationToken cancellationToken)
    {
        return Service.GetPoolAsync(cancellationToken);
    }
}
