using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;

namespace LibplanetConsole.Consoles.Banks;

[Export(typeof(IClientContent))]
[Export(typeof(IClientContentService))]
[Export(typeof(IBankContent))]
[method: ImportingConstructor]
internal sealed class BankClientContent(IClient client)
    : IClientContent, IBankContent, IClientContentService
{
    private readonly RemoteService<IBankService> _bankService = new();

    IRemoteService IClientContentService.RemoteService => _bankService;

    IClient IClientContent.Client => client;

    private IBankService Service => _bankService.Service;

    async Task<BalanceInfo> IBankContent.MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        return await Service.MintAsync(mintOptions.Sign(client), cancellationToken);
    }

    async Task<BalanceInfo> IBankContent.TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        return await Service.TransferAsync(transferOptions.Sign(client), cancellationToken);
    }

    async Task<BalanceInfo> IBankContent.BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        return await Service.BurnAsync(burnOptions.Sign(client), cancellationToken);
    }

    Task<BalanceInfo> IBankContent.GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
        => Service.GetBalanceAsync(address, cancellationToken);

    Task<PoolInfo> IBankContent.GetPoolAsync(CancellationToken cancellationToken)
        => Service.GetPoolAsync(cancellationToken);
}
