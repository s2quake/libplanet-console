using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;

namespace LibplanetConsole.Stakings.Serializations;

public record class RedelegationInfo
{
    public RedelegationInfo(IWorldState worldState, Address delegationAddress)
    {
        if (DelegateCtrl.GetDelegation(worldState, delegationAddress) is not { } delegation)
        {
            throw new ArgumentException("Delegation not found.", nameof(delegationAddress));
        }

        LatestDistributeHeight = delegation.LatestDistributeHeight;
        Address = $"{delegationAddress}";
        DelegatorAddress = $"{delegation.DelegatorAddress}";
        ValidatorAddress = $"{delegation.ValidatorAddress}";
        Share = $"{worldState.GetBalance(delegationAddress, Asset.Share)}";
    }

    public RedelegationInfo()
    {
    }

    public string Address { get; init; } = string.Empty;

    public string DelegatorAddress { get; init; } = string.Empty;

    public string ValidatorAddress { get; init; } = string.Empty;

    public long LatestDistributeHeight { get; init; }

    public string Share { get; init; } = string.Empty;
}
