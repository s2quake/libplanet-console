using Libplanet.Action.State;
using Libplanet.Blockchain;
using LibplanetConsole.Node;
using Microsoft.Extensions.DependencyInjection;
using Nekoyume.Module;

namespace LibplanetConsole.Bank;

public readonly partial record struct BalanceInfo
{
    public BalanceInfo(INode node, Address address)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        var worldState = blockChain.GetWorldState();
        var nativeAddress = address;
        var goldCurrency = worldState.GetGoldCurrency();

        Address = address;
        Gold = $"{worldState.GetBalance(nativeAddress, goldCurrency)}";
        // Governance = $"{worldState.GetBalance(nativeAddress, worldState.GetGoldCurrency())}";
        // Consensus = $"{worldState.GetBalance(nativeAddress, Asset.ConsensusToken)}";
        // Share = $"{worldState.GetBalance(nativeAddress, Asset.Share)}";
    }
}
