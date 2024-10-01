#if LIBPLANET_NODE
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LibplanetConsole.Delegation;

public readonly partial record struct UndelegationInfo
{
    public UndelegationInfo(IWorldState worldState, Address undelegationAddress)
    {
        // if (UndelegateCtrl.GetUndelegation(worldState, undelegationAddress) is not { } undelegation)
        // {
        //     throw new ArgumentException("Undelegation not found.", nameof(undelegationAddress));
        // }

        // Address = undelegationAddress;
        // DelegatorAddress = undelegation.DelegatorAddress;
        // ValidatorAddress = undelegation.ValidatorAddress;
        // Entires = undelegation.UndelegationEntryAddresses.Values.Select(item =>
        // {
        //     return new UndelegationEntryInfo(worldState, item);
        // }).ToArray();
    }
}
#endif // LIBPLANET_NODE
