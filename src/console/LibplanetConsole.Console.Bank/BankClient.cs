using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Services;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;

namespace LibplanetConsole.Console.Bank;

internal sealed class BankClient(IClient client)
    : IClientContent, IBank, IClientContentService
{
    private readonly RemoteService<IBankService> _bankService = new();

    IRemoteService IClientContentService.RemoteService => _bankService;

    IClient IClientContent.Client => client;

    string IClientContent.Name => "Bank";

    private IBankService Service => _bankService.Service;

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

    Task<PoolInfo> IBank.GetPoolAsync(CancellationToken cancellationToken)
        => Service.GetPoolAsync(cancellationToken);
}
