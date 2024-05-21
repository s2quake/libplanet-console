using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;
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
        double amount, CancellationToken cancellationToken)
    {
        var signature = client.Sign(amount);
        return await Service.MintAsync(signature, amount, cancellationToken);
    }

    Task<BalanceInfo> IBankContent.GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
        => Service.GetBalanceAsync(address, cancellationToken);

    Task<PoolInfo> IBankContent.GetPoolAsync(CancellationToken cancellationToken)
        => Service.GetPoolAsync(cancellationToken);
}
