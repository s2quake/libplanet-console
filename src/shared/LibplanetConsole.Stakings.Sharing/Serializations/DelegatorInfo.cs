using Libplanet.Action.State;
using Libplanet.Crypto;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;
using Newtonsoft.Json;
using static Nekoyume.Action.DPoS.Control.ValidatorDelegationSetCtrl;
using static Nekoyume.Action.DPoS.Control.ValidatorSetCtrl;

namespace LibplanetConsole.Stakings.Serializations;

public record class DelegatorInfo
{
    public DelegatorInfo(IWorldState worldState, Address clientAddress)
    {
        Address = $"{clientAddress}";
        Balance = $"{worldState.GetBalance(clientAddress, Asset.GovernanceToken)}";
        Delegations = GetDelegations(worldState, clientAddress);
        Undelegations = GetUndelegations(worldState, clientAddress);
        Redelegations = GetRedelegations(worldState, clientAddress);
    }

    public DelegatorInfo()
    {
    }

    [JsonIgnore]
    public string Address { get; init; } = string.Empty;

    public string Balance { get; init; } = string.Empty;

    public DelegationInfo[] Delegations { get; init; } = [];

    public UndelegationInfo[] Undelegations { get; init; } = [];

    public RedelegationInfo[] Redelegations { get; init; } = [];

    private static DelegationInfo[] GetDelegations(IWorldState worldState, Address clientAddress)
    {
        var validatorSet = GetValidatorSet(worldState, ReservedAddress.BondedValidatorSet)!;
        var validators = validatorSet.Set;
        var delegationList = new List<DelegationInfo>();
        foreach (var validator in validators)
        {
            var validatorAddress = validator.ValidatorAddress;
            var delegationSet = GetValidatorDelegationSet(worldState, validatorAddress)!;
            var delegationAddreses = delegationSet.Set;
            foreach (var delegationAddress in delegationAddreses)
            {
                var delegation = DelegateCtrl.GetDelegation(worldState, delegationAddress)!;
                if (delegation.DelegatorAddress == clientAddress)
                {
                    delegationList.Add(new DelegationInfo(worldState, delegationAddress));
                }
            }
        }

        return [.. delegationList];
    }

    private static UndelegationInfo[] GetUndelegations(
        IWorldState worldState, Address clientAddress)
    {
        return UndelegateCtrl.GetUndelegationsByDelegator(worldState, clientAddress)
                             .Select(item => new UndelegationInfo(worldState, item.Address))
                             .ToArray();
    }

    private static RedelegationInfo[] GetRedelegations(
        IWorldState worldState, Address clientAddress)
    {
        return RedelegateCtrl.GetRedelegationsByDelegator(worldState, clientAddress)
                             .Select(item => new RedelegationInfo(worldState, item.Address))
                             .ToArray();
    }
}
