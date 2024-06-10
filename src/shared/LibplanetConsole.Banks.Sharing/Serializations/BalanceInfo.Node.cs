#if LIBPLANET_NODE
using Libplanet.Action.State;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Nodes;
using Nekoyume.Action.DPoS.Misc;
using Nekoyume.Module;

namespace LibplanetConsole.Banks.Serializations;

public readonly partial record struct BalanceInfo
{
    public BalanceInfo(INode node, AppAddress address)
    {
        var blockChain = node.GetService<BlockChain>();
        var worldState = blockChain.GetWorldState();
        var nativeAddress = (Address)address;

        Address = address;
        Governance = $"{worldState.GetBalance(nativeAddress, worldState.GetGoldCurrency())}";
        Consensus = $"{worldState.GetBalance(nativeAddress, Asset.ConsensusToken)}";
        Share = $"{worldState.GetBalance(nativeAddress, Asset.Share)}";
    }
}
#endif // LIBPLANET_NODE
