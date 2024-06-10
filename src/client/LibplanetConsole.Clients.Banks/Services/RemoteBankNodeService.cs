using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Banks.Services;
using LibplanetConsole.Common;
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
                Address = (Address)address,
                Amount = ncg,
            },
        };
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetBalanceAsync(address, cancellationToken);
    }

    public async Task<BalanceInfo> TransferAsync(
        double amount, AppAddress targetAddress, CancellationToken cancellationToken)
    {
        var address = client.Address;
        var ncg = AssetUtility.GetNCG(amount);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.TransferAssetAction
            {
                TargetAddress = (Address)targetAddress,
                Amount = ncg,
            },
        };
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetBalanceAsync(address, cancellationToken);
    }

    public async Task<BalanceInfo> BurnAsync(
        double amount, CancellationToken cancellationToken)
    {
        var address = client.Address;
        var ncg = AssetUtility.GetNCG(amount);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.BurnAssetAction
            {
                Address = (Address)address,
                Amount = ncg,
            },
        };
        await client.SendTransactionAsync(actions, cancellationToken);
        return await Service.GetBalanceAsync(address, cancellationToken);
    }

    public Task<BalanceInfo> GetBalanceAsync(
        AppAddress address, CancellationToken cancellationToken)
        => Service.GetBalanceAsync(address, cancellationToken);

    public Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
        => Service.GetPoolAsync(cancellationToken);
}
