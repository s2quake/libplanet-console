using Libplanet.Action.State;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using Nekoyume;
using Nekoyume.Model.State;
using Nekoyume.Module;
using Nekoyume.Module.ValidatorDelegation;
using Nekoyume.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation;

[ActionType("initialize_states")]
public sealed class InitializeStates : ActionBase
{
    public Validator[] Validators { get; set; } = [];

    public Currency GoldCurrency { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values
            .Add("validator_set", new List(Validators.Select(item => item.Bencoded)))
            .Add("gold_currency", GoldCurrency.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        if (values["validator_set"] is List list)
        {
            Validators = list.Select(item => new Validator((Dictionary)item)).ToArray();
        }

        GoldCurrency = new Currency(values["gold_currency"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;

        var goldCurrency = GoldCurrency;
        var currencyState = new GoldCurrencyState(goldCurrency);
        world = world
            .SetLegacyState(GoldCurrencyState.Address, currencyState.Serialize())
            .SetLegacyState(Addresses.GoldDistribution, new List().Serialize());

        if (currencyState.InitialSupply > 0)
        {
            world = world.MintAsset(
                context,
                GoldCurrencyState.Address,
                currencyState.Currency * currencyState.InitialSupply);
        }

        var repository = new ValidatorRepository(world, context);
        var validators = Validators;
        var validatorList = new List();
        foreach (var validator in validators)
        {
            var validatorDelegatee = new ValidatorDelegatee(
                validator.OperatorAddress, validator.PublicKey, repository.World.GetGoldCurrency(), repository);
            var delegationFAV = validatorDelegatee.MinSelfDelegation;

            repository.SetValidatorDelegatee(validatorDelegatee);
            var validatorDelegator = repository.GetValidatorDelegator(validator.OperatorAddress);
            repository.TransferAsset(
                GoldCurrencyState.Address, validatorDelegator.DelegationPoolAddress, delegationFAV);
            validatorDelegator.Delegate(validatorDelegatee, delegationFAV, context.BlockIndex);

            validatorList = validatorList.Add(validator.Bencoded);
        }

        repository.SetAbstainHistory(new());
        repository.SetValidatorList(new ValidatorList(validatorList));
        world = repository.World;

        return world;
    }
}
