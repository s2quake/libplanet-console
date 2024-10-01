using Libplanet.Action.State;
using Libplanet.Blockchain;
using LibplanetConsole.Bank;
using LibplanetConsole.Bank.Actions;
using Microsoft.Extensions.DependencyInjection;
using Nekoyume.Model.State;
using Nekoyume.Module;

namespace LibplanetConsole.Node.Bank;

internal sealed class Bank(INode node) : IBank
{
    public async Task<BalanceInfo> MintAsync(
        decimal amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new MintAction
            {
                Address = address,
                Amount = amount,
            },
        };
        await node.SendTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> TransferAsync(
        Address targetAddress, decimal amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new TransferAction
            {
                TargetAddress = targetAddress,
                Amount = amount,
            },
        };
        await node.SendTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> BurnAsync(
        decimal amount, CancellationToken cancellationToken)
    {
        var address = node.Address;
        var actions = new IAction[]
        {
            new BurnAction
            {
                Address = address,
                Amount = amount,
            },
        };
        await node.SendTransactionAsync(actions, cancellationToken);
        return new BalanceInfo(node, address);
    }

    public async Task<BalanceInfo> GetBalanceAsync(
        Address address, CancellationToken cancellationToken)
    {
        return await Task.Run(() => new BalanceInfo(node, address));
    }

    public async Task<PoolInfo> GetPoolAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        var worldState = blockChain.GetWorldState();
        return await Task.Run(() => new PoolInfo(worldState));
    }

    public Task<decimal> GetInitialSupplyAsync(CancellationToken cancellationToken)
    {
        decimal Action()
        {
            var blockChain = node.GetRequiredService<BlockChain>();
            var worldState = blockChain.GetWorldState();
            var goldCurrency = worldState.GetGoldCurrency();
            if (goldCurrency.MaximumSupply is { } maximumSupply)
            {
                return decimal.Parse(maximumSupply.GetQuantityString());
            }

            return 0;
        }

        return Task.Run(Action, cancellationToken);
    }

    public Task<decimal> GetSupplyAsync(CancellationToken cancellationToken)
    {
        decimal Action()
        {
            var blockChain = node.GetRequiredService<BlockChain>();
            var worldState = blockChain.GetWorldState();
            var goldCurrency = worldState.GetGoldCurrency();
            var balance = worldState.GetBalance(GoldCurrencyState.Address, goldCurrency);
            return decimal.Parse(balance.GetQuantityString());
        }

        return Task.Run(Action, cancellationToken);
    }
}
