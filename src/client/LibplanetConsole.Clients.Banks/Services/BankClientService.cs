using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Banks.Services;

[Export(typeof(IBankService))]
[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class BankClientService(IClient client, IBankClient bankClient)
    : LocalService<IBankService>, IBankService
{
    public Task<BalanceInfo> MintAsync(
        MintOptions mintOptions, CancellationToken cancellationToken)
    {
        mintOptions.Verify(client);
        return bankClient.MintAsync(mintOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> TransferAsync(
        TransferOptions transferOptions, CancellationToken cancellationToken)
    {
        transferOptions.Verify(client);
        return bankClient.TransferAsync(
            amount: transferOptions.Amount,
            targetAddress: transferOptions.TargetAddress,
            cancellationToken: cancellationToken);
    }

    public Task<BalanceInfo> BurnAsync(
        BurnOptions burnOptions, CancellationToken cancellationToken)
    {
        burnOptions.Verify(client);
        return bankClient.BurnAsync(burnOptions.Amount, cancellationToken);
    }

    public Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken)
        => bankClient.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => bankClient.GetPoolAsync(cancellationToken);
}
