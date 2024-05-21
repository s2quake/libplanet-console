using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Banks;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Clients;
using LibplanetConsole.Common.Services;

namespace LibplanetConsole.Clients.Banks.Services;

[Export]
[Export(typeof(IBankClient))]
[Export(typeof(IRemoteService))]
[method: ImportingConstructor]
internal sealed class RemoteBankNodeService(IClient client)
    : RemoteService<IBankService>, IBankClient
{
    public async Task<BalanceInfo> MintAsync(
        double amount, CancellationToken cancellationToken)
    {
        var address = client.Address;
        var ncg = AssetUtility.GetNCG(amount);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.MintAssetAction
            {
                Address = address,
                Amount = ncg,
            },
        };
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetBalanceAsync(address, cancellationToken);
    }

    public Task<BalanceInfo> GetBalanceAsync(Address address, CancellationToken cancellationToken)
        => Service.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => Service.GetPoolAsync(cancellationToken);
}
