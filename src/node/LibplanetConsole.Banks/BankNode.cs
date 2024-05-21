using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Crypto;
using LibplanetConsole.Banks.Serializations;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.Banks;

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
                Address = address,
                Amount = AssetUtility.GetNCG(amount),
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
        var worldState = node.BlockChain.GetWorldState();
        var balanceInfo = new BalanceInfo(worldState, address);
        return balanceInfo;
    }

    public async Task<BalanceInfo> GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
    {
        var worldState = node.BlockChain.GetWorldState();
        return await Task.Run(() => new BalanceInfo(worldState, address));
    }

    public async Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
    {
        var worldState = node.BlockChain.GetWorldState();
        return await Task.Run(() => new PoolInfo(worldState));
    }
}
