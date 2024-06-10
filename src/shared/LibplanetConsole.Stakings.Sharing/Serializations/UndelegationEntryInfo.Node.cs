#if LIBPLANET_NODE
using Libplanet.Action.State;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS;
using Nekoyume.Action.DPoS.Model;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct UndelegationEntryInfo
{
    public UndelegationEntryInfo(IWorldState worldState, AppAddress entryAddress)
    {
        var entryValue = worldState.GetDPoSState(entryAddress)!;
        var entry = new UndelegationEntry(entryValue);
        InitialConsensusToken = $"{entry.InitialConsensusToken}";
        UnbondingConsensusToken = $"{entry.UnbondingConsensusToken}";
        CreationHeight = entry.CreationHeight;
    }
}
#endif // LIBPLANET_NODE
