using Libplanet.Action.State;
using Libplanet.Types.Consensus;
using LibplanetConsole.Common;
using Nekoyume;
using Nekoyume.Action.Guild.Migration.LegacyModels;
using Nekoyume.Model.Guild;
using Nekoyume.Model.Stake;
using Nekoyume.Model.State;
using Nekoyume.Module;
using Nekoyume.Module.Guild;
using Nekoyume.Module.ValidatorDelegation;
using Nekoyume.ValidatorDelegation;

namespace LibplanetConsole.Node.Guild;

[ActionType("initialize_world")]
public sealed class InitializeWorld : ActionBase
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
            .SetLegacyState(Addresses.GoldDistribution, new List().Serialize())
            .SetDelegationMigrationHeight(0);

        if (currencyState.InitialSupply > 0)
        {
            world = world.MintAsset(
                context,
                GoldCurrencyState.Address,
                currencyState.Currency * currencyState.InitialSupply);
        }

        var validators = Validators;
        foreach (var validator in validators)
        {
            var delegationFAV = FungibleAssetValue.FromRawValue(
                ValidatorDelegatee.ValidatorDelegationCurrency, validator.Power);
            world = world.MintAsset(
                context, StakeState.DeriveAddress(validator.OperatorAddress), delegationFAV);

            var validatorRepository = new ValidatorRepository(world, context);
            var validatorDelegatee = validatorRepository.CreateValidatorDelegatee(
                validator.PublicKey, ValidatorDelegatee.DefaultCommissionPercentage);
            var validatorDelegator = validatorRepository.GetValidatorDelegator(
                validator.OperatorAddress);
            validatorDelegatee.Bond(validatorDelegator, delegationFAV, context.BlockIndex);

            var guildRepository = new GuildRepository(validatorRepository);
            var guildDelegatee = guildRepository.CreateGuildDelegatee(validator.OperatorAddress);
            var guildDelegator = guildRepository.GetGuildDelegator(validator.OperatorAddress);
            guildDelegator.Delegate(guildDelegatee, delegationFAV, context.BlockIndex);
            world = guildRepository.World;
        }

        var repository = new ValidatorRepository(world, context);
        repository.SetAbstainHistory(new());
        world = repository.World;

        return world;
    }
}
