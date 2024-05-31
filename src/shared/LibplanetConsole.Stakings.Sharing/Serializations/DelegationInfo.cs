using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;
using Newtonsoft.Json;

namespace LibplanetConsole.Stakings.Serializations;

public record class DelegationInfo
{
    public DelegationInfo(IWorldState worldState, Address delegationAddress)
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

    public DelegationInfo()
    {
    }

    [JsonIgnore]
    public string Address { get; init; } = string.Empty;

    public string DelegatorAddress { get; init; } = string.Empty;

    public string ValidatorAddress { get; init; } = string.Empty;

    public long LatestDistributeHeight { get; init; }

    public string Share { get; init; } = string.Empty;
}
