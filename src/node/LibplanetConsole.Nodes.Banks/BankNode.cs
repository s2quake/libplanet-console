using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Banks;

[Export(typeof(IBankNode))]
[Export]
[method: ImportingConstructor]
internal sealed class BankNode(INode node) : IBankNode
{
    public async Task<BalanceInfo> MintAsync(
        double amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.MintAssetAction
            {
                Address = (Address)address,
                Amount = AssetUtility.GetNCG(amount),
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> TransferAsync(
        AppAddress targetAddress, double amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.TransferAssetAction
            {
                TargetAddress = (Address)targetAddress,
                Amount = AssetUtility.GetNCG(amount),
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> BurnAsync(
        double amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.BurnAssetAction
            {
                Address = (Address)address,
                Amount = AssetUtility.GetNCG(amount),
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> GetBalanceAsync(
        AppAddress address, CancellationToken cancellationToken)
    {
        return await Task.Run(() => new BalanceInfo(node, address));
    }

    public async Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        var worldState = blockChain.GetWorldState();
        return await Task.Run(() => new PoolInfo(worldState));
    }
}
