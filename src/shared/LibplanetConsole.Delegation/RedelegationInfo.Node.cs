#if LIBPLANET_NODE
using Libplanet.Action.State;
using LibplanetConsole.Common;

namespace LibplanetConsole.Delegation;

public readonly partial record struct RedelegationInfo
{
    public RedelegationInfo(IWorldState worldState, Address delegationAddress)
    {
        // if (DelegateCtrl.GetDelegation(worldState, delegationAddress) is not { } delegation)
        // {
        //     throw new ArgumentException("Delegation not found.", nameof(delegationAddress));
        // }

        // LatestDistributeHeight = delegation.LatestDistributeHeight;
        // Address = delegationAddress;
        // DelegatorAddress = delegation.DelegatorAddress;
        // ValidatorAddress = delegation.ValidatorAddress;
        // Share = $"{worldState.GetBalance(delegationAddress, Asset.Share)}";
    }
}
#endif // LIBPLANET_NODE
