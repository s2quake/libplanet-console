using Libplanet.Action.State;
using Libplanet.Crypto;
using Libplanet.Types.Assets;
using Nekoyume.Action.DPoS.Control;
using Nekoyume.Action.DPoS.Misc;
using Nekoyume.Action.DPoS.Model;

namespace LibplanetConsole.Stakings.Serializations;

public record class ValidatorInfo
{
    public ValidatorInfo(IWorldState worldState, Address nodeAddress)
    {
        var validatorAddress = Validator.DeriveAddress(nodeAddress);
        Address = $"{validatorAddress}";
        OperatorAddress = $"{nodeAddress}";
        if (ValidatorCtrl.GetValidator(worldState, validatorAddress) is { } validator)
        {
            Jailed = validator.Jailed;
            Status = $"{validator.Status}";
            DelegatorShares = validator.DelegatorShares.ToString();
            Power = $"{GetPower(worldState, validatorAddress)}";
        }

        Delegations = GetDelegations(worldState, validatorAddress);
        Undelegations = GetUndelections(worldState, validatorAddress);

        Balance = $"{worldState.GetBalance(nodeAddress, Asset.GovernanceToken)}";
        Identifier = $"{nodeAddress}"[0..8];
    }

    public ValidatorInfo()
    {
    }

    public string Address { get; init; } = string.Empty;

    public string OperatorAddress { get; init; } = string.Empty;

    public bool Jailed { get; init; }

    public string Status { get; init; } = $"{BondingStatus.Unbonded}";

    public string Identifier { get; init; } = string.Empty;

    public string DelegatorShares { get; init; } = string.Empty;

    public string Power { get; init; } = string.Empty;

    public string Balance { get; init; } = string.Empty;

    public DelegationInfo[] Delegations { get; init; } = [];

    public UndelegationInfo[] Undelegations { get; init; } = [];

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
