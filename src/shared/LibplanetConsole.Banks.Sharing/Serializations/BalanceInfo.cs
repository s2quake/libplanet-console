using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS.Misc;

namespace LibplanetConsole.Banks.Serializations;

public record class BalanceInfo
{
    public BalanceInfo(IWorldState worldState, Address address)
    {
        Address = $"{address}";
        Governance = $"{worldState.GetBalance(address, Asset.GovernanceToken)}";
        Consensus = $"{worldState.GetBalance(address, Asset.ConsensusToken)}";
        Share = $"{worldState.GetBalance(address, Asset.Share)}";
        Identifier = $"{address}"[0..8];
    }

    public BalanceInfo()
    {
    }

    public string Address { get; init; } = string.Empty;

    public string Governance { get; init; } = string.Empty;

    public string Consensus { get; init; } = string.Empty;

    public string Share { get; init; } = string.Empty;

    public string Identifier { get; init; } = string.Empty;
}
