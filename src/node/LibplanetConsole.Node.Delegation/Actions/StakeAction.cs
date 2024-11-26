using System.Collections.Immutable;
using System.Numerics;
using Lib9c;
using Libplanet.Action.State;
using Nekoyume.Action;
using Nekoyume.Exceptions;
using Nekoyume.Extensions;
using Nekoyume.Model.Guild;
using Nekoyume.Model.Stake;
using Nekoyume.Model.State;
using Nekoyume.Module;
using Nekoyume.Module.Guild;
using Nekoyume.TypedAddress;
using Nekoyume.ValidatorDelegation;
using Serilog;
using static Lib9c.SerializeKeys;

namespace LibplanetConsole.Node.Delegation.Actions;

[ActionType("stake_action")]
public sealed class StakeAction : GameAction
{
    public const long MinimumRequiredGold = 100;

    public StakeAction()
    {
    }

    public StakeAction(BigInteger amount)
    {
        Amount = amount >= 0
            ? amount
            : throw new ArgumentOutOfRangeException(nameof(amount));
    }

    internal BigInteger Amount { get; set; }

    protected override IImmutableDictionary<string, IValue> PlainValueInternal =>
        ImmutableDictionary<string, IValue>.Empty.Add(AmountKey, (IValue)(Integer)Amount);

    public override IWorld Execute(IActionContext context)
    {
        var started = DateTimeOffset.UtcNow;
        GasTracer.UseGas(1);
        IWorld states = context.PreviousState;

        // NOTE: Restrict staking if there is a monster collection until now.
        if (states.GetAgentState(context.Signer) is { } agentState &&
            states.TryGetLegacyState(
                MonsterCollectionState.DeriveAddress(
                    context.Signer,
                    agentState.MonsterCollectionRound),
                out Dictionary _))
        {
            throw new MonsterCollectionExistingException();
        }

        // NOTE: When the amount is less than 0.
        if (Amount < 0)
        {
            throw new InvalidOperationException("The amount must be greater than or equal to 0.");
        }

        var addressesHex = GetSignerAndOtherAddressesHex(context, context.Signer);
        var minimumRequiredGold = 100;
        if (Amount != 0 && Amount < MinimumRequiredGold)
        {
            throw new InvalidOperationException(
                $"The amount must be greater than or equal to {minimumRequiredGold}.");
        }

        var stakeStateAddress = LegacyStakeState.DeriveAddress(context.Signer);
        var currency = states.GetGoldCurrency();
        var currentBalance = states.GetBalance(context.Signer, currency);
        var stakedBalance = states.GetBalance(stakeStateAddress, currency);
        var targetStakeBalance = currency * Amount;
        if (currentBalance + stakedBalance < targetStakeBalance)
        {
            throw new NotEnoughFungibleAssetValueException(
                context.Signer.ToHex(),
                Amount,
                currentBalance);
        }

        var latestStakeContract = new Contract(
            "StakeRegularFixedRewardSheet_V2",
            "StakeRegularRewardSheet_V2",
            50400,
            201600);
        if (!states.TryGetStakeState(context.Signer, out var stakeStateV2))
        {
            // NOTE: Cannot withdraw staking.
            if (Amount == 0)
            {
                throw new StateNullException(ReservedAddresses.LegacyAccount, stakeStateAddress);
            }

            states = ContractNewStake(
                context,
                states,
                stakeStateAddress,
                stakedBalance: currency * 0,
                targetStakeBalance,
                latestStakeContract);
            Log.Debug(
                "{AddressesHex}Stake Total Executed Time: {Elapsed}",
                addressesHex,
                DateTimeOffset.UtcNow - started);
            return states;
        }

        // NOTE: Cannot anything if staking state is claimable.
        if (stakeStateV2.ClaimableBlockIndex <= context.BlockIndex)
        {
            throw new StakeExistingClaimableException();
        }

        // NOTE: When the staking state is locked up.
        if (stakeStateV2.CancellableBlockIndex > context.BlockIndex
            && targetStakeBalance < stakedBalance)
        {
            throw new RequiredBlockIndexException();
        }

        if (stakeStateV2.StateVersion == 2)
        {
            if (!StakeStateUtils.TryMigrateV2ToV3(
                context, states, stakeStateAddress, stakeStateV2, out var result))
            {
                throw new InvalidOperationException("Failed to migration. Unexpected situation.");
            }

            states = result.Value.world;
        }

        // NOTE: Contract a new staking.
        states = ContractNewStake(
            context,
            states,
            stakeStateAddress,
            stakedBalance,
            targetStakeBalance,
            latestStakeContract);
        Log.Debug(
            "{AddressesHex}Stake Total Executed Time: {Elapsed}",
            addressesHex,
            DateTimeOffset.UtcNow - started);
        return states;
    }

    protected override void LoadPlainValueInternal(
        IImmutableDictionary<string, IValue> plainValue)
    {
        Amount = plainValue[AmountKey].ToBigInteger();
    }

    private static IWorld ContractNewStake(
        IActionContext context,
        IWorld state,
        Address stakeStateAddr,
        FungibleAssetValue stakedBalance,
        FungibleAssetValue targetStakeBalance,
        Contract latestStakeContract)
    {
        var stakeStateValue = new StakeState(latestStakeContract, context.BlockIndex).Serialize();
        var additionalBalance = targetStakeBalance - stakedBalance;
        var height = context.BlockIndex;
        var agentAddress = new AgentAddress(context.Signer);

        if (additionalBalance.Sign > 0)
        {
            var gg = GetGuildCoinFromNCG(additionalBalance);
            state = state
                .TransferAsset(context, context.Signer, stakeStateAddr, additionalBalance)
                .MintAsset(context, stakeStateAddr, gg);

            var guildRepository = new GuildRepository(state, context);
            if (guildRepository.TryGetGuildParticipant(agentAddress, out var guildParticipant))
            {
                var guild = guildRepository.GetGuild(guildParticipant.GuildAddress);
                guildParticipant.Delegate(guild, gg, height);
                state = guildRepository.World;
            }
        }
        else if (additionalBalance.Sign < 0)
        {
            var gg = GetGuildCoinFromNCG(-additionalBalance);

            var guildRepository = new GuildRepository(state, context);

            if (guildRepository.TryGetGuildParticipant(agentAddress, out var guildParticipant))
            {
                var guild = guildRepository.GetGuild(guildParticipant.GuildAddress);
                var guildDelegatee = guildRepository.GetGuildDelegatee(guild.ValidatorAddress);
                var share = guildDelegatee.ShareFromFAV(gg);

                var guildDelegator = guildRepository.GetGuildDelegator(agentAddress);
                guildDelegatee.Unbond(guildDelegator, share, height);

                var validatorRepository = new ValidatorRepository(guildRepository);
                var validatorDelegatee = validatorRepository.GetValidatorDelegatee(
                    guild.ValidatorAddress);
                var validatorDelegator = validatorRepository.GetValidatorDelegator(guild.Address);
                validatorDelegatee.Unbond(validatorDelegator, share, height);

                state = validatorRepository.World;
                state = state.BurnAsset(context, guildDelegatee.DelegationPoolAddress, gg);
            }
            else
            {
                state = state.BurnAsset(context, stakeStateAddr, gg);
            }

            state = state
                .TransferAsset(context, stakeStateAddr, context.Signer, -additionalBalance);

            if ((stakedBalance + additionalBalance).Sign == 0)
            {
                return state.MutateAccount(
                    ReservedAddresses.LegacyAccount,
                    state => state.RemoveState(stakeStateAddr));
            }
        }

        return state.SetLegacyState(stakeStateAddr, stakeStateValue);
    }

    private static FungibleAssetValue GetGuildCoinFromNCG(FungibleAssetValue balance)
    {
        return FungibleAssetValue.Parse(
            Currencies.GuildGold, balance.GetQuantityString(true));
    }
}
