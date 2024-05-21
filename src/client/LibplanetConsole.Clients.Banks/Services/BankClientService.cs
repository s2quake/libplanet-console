using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Banks.Services;

[Export(typeof(IBankService))]
[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class BankClientService(IClient client, IBankClient bankClient)
    : LocalService<IBankService>, IBankService
{
    public Task<BalanceInfo> MintAsync(
        byte[] signature, double amount, CancellationToken cancellationToken)
    {
        if (client.Verify(amount, signature) == true)
        {
            return bankClient.MintAsync(amount, cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signature));
    }

    public Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken)
        => bankClient.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => bankClient.GetPoolAsync(cancellationToken);
}
