#if LIBPLANET_NODE
using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;
using Nekoyume.Action.DPoS.Model;
using Nekoyume.Module;

namespace LibplanetConsole.Stakings.Serializations;

public readonly partial record struct ValidatorInfo
{
    public ValidatorInfo(IWorldState worldState, AppAddress nodeAddress)
    {
        var validatorAddress = Validator.DeriveAddress(nodeAddress);
        Address = validatorAddress;
        OperatorAddress = nodeAddress;
        if (ValidatorCtrl.GetValidator(worldState, validatorAddress) is { } validator)
        {
            Jailed = validator.Jailed;
            Status = $"{validator.Status}";
            DelegatorShares = validator.DelegatorShares.ToString();
            Power = $"{GetPower(worldState, validatorAddress)}";
        }

        Delegations = GetDelegations(worldState, validatorAddress);
        Undelegations = GetUndelections(worldState, validatorAddress);

        Balance = $"{worldState.GetBalance(nodeAddress, worldState.GetGoldCurrency())}";
    }

    private static FungibleAssetValue GetPower(IWorldState worldState, Address validatorAddress)
    {
        if (ValidatorPowerIndexCtrl.GetValidatorPowerIndex(worldState) is { } powerIndex)
        {
            return powerIndex.Index.First(item => item.ValidatorAddress == validatorAddress)
                    .ConsensusToken;
        }

        throw new ArgumentException("Validator power index not found.", nameof(validatorAddress));
    }

    private static DelegationInfo[] GetDelegations(IWorldState worldState, Address validatorAddress)
    {
        var delegationSet = ValidatorDelegationSetCtrl.GetValidatorDelegationSet(
            worldState,
            validatorAddress);
        if (delegationSet is not null)
        {
            return delegationSet.Set.Select(item => new DelegationInfo(worldState, item)).ToArray();
        }

        return [];
    }

    private static UndelegationInfo[] GetUndelections(
        IWorldState worldState, Address validatorAddress)
    {
        return UndelegateCtrl.GetUndelegations(worldState, validatorAddress)
                             .Select(item => new UndelegationInfo(worldState, item.Address))
                             .ToArray();
    }
}
#endif // LIBPLANET_NODE
