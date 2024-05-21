using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS;
using Nekoyume.Action.DPoS.Model;

namespace LibplanetConsole.Stakings.Serializations;

public record class UndelegationEntryInfo
{
    public UndelegationEntryInfo(IWorldState worldState, Address entryAddress)
    {
        var entryValue = worldState.GetDPoSState(entryAddress)!;
        var entry = new UndelegationEntry(entryValue);
        InitialConsensusToken = $"{entry.InitialConsensusToken}";
        UnbondingConsensusToken = $"{entry.UnbondingConsensusToken}";
        CreationHeight = entry.CreationHeight;
    }

    public UndelegationEntryInfo()
    {
    }

    public string InitialConsensusToken { get; init; } = string.Empty;

    public string UnbondingConsensusToken { get; init; } = string.Empty;

    public long CreationHeight { get; init; }
}
