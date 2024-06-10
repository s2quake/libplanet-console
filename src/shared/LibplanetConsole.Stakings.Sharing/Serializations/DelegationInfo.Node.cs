#if LIBPLANET_NODE
using Libplanet.Action.State;
using LibplanetConsole.Common;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct DelegationInfo
{
    public DelegationInfo(IWorldState worldState, AppAddress delegationAddress)
    {
        if (DelegateCtrl.GetDelegation(worldState, delegationAddress) is not { } delegation)
        {
            throw new ArgumentException("Delegation not found.", nameof(delegationAddress));
        }

        LatestDistributeHeight = delegation.LatestDistributeHeight;
        Address = delegationAddress;
        DelegatorAddress = delegation.DelegatorAddress;
        ValidatorAddress = delegation.ValidatorAddress;
        Share = $"{worldState.GetBalance(delegationAddress, Asset.Share)}";
    }
}
#endif // LIBPLANET_NODE
