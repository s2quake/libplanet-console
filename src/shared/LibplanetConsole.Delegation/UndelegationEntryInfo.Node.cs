#if LIBPLANET_NODE
using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Delegation;

public readonly partial record struct UndelegationEntryInfo
{
    public UndelegationEntryInfo(IWorldState worldState, Address entryAddress)
    {
        // var entryValue = worldState.GetDPoSState(entryAddress)!;
        // var entry = new UndelegationEntry(entryValue);
        // InitialConsensusToken = $"{entry.InitialConsensusToken}";
        // UnbondingConsensusToken = $"{entry.UnbondingConsensusToken}";
        // CreationHeight = entry.CreationHeight;
    }
}
#endif // LIBPLANET_NODE
