using Lib9c;
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
    public required Validator[] Validators { get; set; } = [];

    public required Currency GoldCurrency { get; set; }

    public required Address GenesisAddress { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values
            .Add("validator_set", new List(Validators.Select(item => item.Bencoded)))
            .Add("gold_currency", GoldCurrency.Serialize())
            .Add("genesis_address", GenesisAddress.Serialize());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        if (values["validator_set"] is List list)
        {
            Validators = list.Select(item => new Validator((Dictionary)item)).ToArray();
        }

        GoldCurrency = new Currency(values["gold_currency"]);
        GenesisAddress = new Address(values["genesis_address"]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;

        var goldCurrency = GoldCurrency;
        var currencyState = new GoldCurrencyState(goldCurrency);
        var adminState = new AdminState(GenesisAddress, long.MaxValue);
        world = world
            .SetLegacyState(GoldCurrencyState.Address, currencyState.Serialize())
            .SetLegacyState(Addresses.GoldDistribution, new List().Serialize())
            .SetLegacyState(Addresses.Admin, adminState.Serialize())
            .MintAsset(context, GoldCurrencyState.Address, Currencies.GuildGold * 100_000_000_000)
            .MintAsset(context, GoldCurrencyState.Address, Currencies.Mead * 100_000_000_000)
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
            var validatorAddress = validator.OperatorAddress;
            var validatorStakeAddress = StakeState.DeriveAddress(validatorAddress);
            var stakeContract = new Contract(
                "StakeRegularFixedRewardSheet_V2",
                "StakeRegularRewardSheet_V2",
                2,
                4);
            var stakeState = new StakeState(stakeContract, context.BlockIndex);
            var delegationFAV = FungibleAssetValue.FromRawValue(
                ValidatorDelegatee.ValidatorDelegationCurrency, validator.Power);
            var balance = currencyState.Currency * 1000;
            world = world
                .MintAsset(context, validatorStakeAddress, delegationFAV)
                .TransferAsset(context, GoldCurrencyState.Address, validatorAddress, balance);

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
            world = world.SetLegacyState(validatorStakeAddress, stakeState.Serialize());
        }

        var repository = new ValidatorRepository(world, context);
        repository.SetAbstainHistory(new());
        world = repository.World;

        return world;
    }
}
