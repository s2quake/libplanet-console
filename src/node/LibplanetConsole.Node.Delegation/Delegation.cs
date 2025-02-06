using System.Numerics;
using Lib9c;
using Libplanet.Action.State;
using LibplanetConsole.Bank;
using LibplanetConsole.BlockChain;
using LibplanetConsole.Delegation;
using LibplanetConsole.Node.Delegation.Actions;
using Nekoyume.Action;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.Guild;
using Nekoyume.Model.Stake;
using Nekoyume.Model.State;
using Nekoyume.Module;
using Nekoyume.Module.Guild;
using Nekoyume.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation;

internal sealed class Delegation(
    INode node, IBlockChain blockChain, ICurrencyCollection currencies)
    : NodeContentBase(nameof(Delegation)), IDelegation
{
    private Currency? _goldCurrency;

    public Currency GoldCurrency => _goldCurrency
        ?? throw new InvalidOperationException("There is no currency information.");

    public async Task StakeAsync(long ncg, CancellationToken cancellationToken)
    {
        var stake = new Stake(ncg);
        await node.SendTransactionAsync([stake], cancellationToken);
    }

    public async Task PromoteAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        var publicKey = node.PublicKey;
        var promoteValidator = new PromoteValidator(publicKey, guildGold);
        await node.SendTransactionAsync([promoteValidator], cancellationToken);
    }

    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var unjailValidator = new UnjailValidator();
        await node.SendTransactionAsync([unjailValidator], cancellationToken);
    }

    public async Task DelegateAsync(
        FungibleAssetValue guildGold, CancellationToken cancellationToken)
    {
        var delegateAction = new DelegateValidator(guildGold);
        await node.SendTransactionAsync([delegateAction], cancellationToken);
    }

    public async Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken)
    {
        var undelegateAction = new UndelegateValidator(share);
        await node.SendTransactionAsync([undelegateAction], cancellationToken);
    }

    public async Task SetCommissionAsync(long commission, CancellationToken cancellationToken)
    {
        var setValidatorCommissionAction = new SetValidatorCommission(commission);
        await node.SendTransactionAsync([setValidatorCommissionAction], cancellationToken);
    }

    public async Task ClaimAsync(CancellationToken cancellationToken)
    {
        var claimValidatorRewardSelfAction = new ClaimValidatorRewardSelf();
        await node.SendTransactionAsync([claimValidatorRewardSelfAction], cancellationToken);
    }

    public async Task SlashAsync(long slashFactor, CancellationToken cancellationToken)
    {
        var slashAction = new SlashAction(slashFactor);
        await node.SendTransactionAsync([slashAction], cancellationToken);
    }

    public Task<DelegateeInfo> GetDelegateeInfoAsync(
        Address address, CancellationToken cancellationToken)
        => Task.FromResult(GetDelegateeInfo(address));

    public Task<DelegatorInfo> GetDelegatorInfoAsync(
        Address address, CancellationToken cancellationToken)
        => Task.FromResult(GetDelegatorInfo(address));

    public Task<StakeInfo> GetStakeInfoAsync(
        Address address, CancellationToken cancellationToken)
        => Task.FromResult(GetStakeInfo(address));

    public DelegateeInfo GetDelegateeInfo(Address address)
    {
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var repository = new ValidatorRepository(world, new DummayActionContext());
        var delegatee = repository.GetDelegatee(address);

        var delegateeInfo = new DelegateeInfo
        {
            Power = BigIntegerUtility.ToString(delegatee.Power),
            TotalShare = BigIntegerUtility.ToString(delegatee.Metadata.TotalShares),
            IsJailed = delegatee.Jailed,
            JailedUntil = delegatee.Metadata.JailedUntil,
            Commission = (long)delegatee.CommissionPercentage,
            Tombstoned = delegatee.Metadata.Tombstoned,
            IsActive = delegatee.IsActive,
        };

        return delegateeInfo;
    }

    public DelegatorInfo GetDelegatorInfo(Address address)
    {
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var guildRepository = new GuildRepository(world, new DummayActionContext());
        var lastDistributeHeight = -1L;
        var share = new BigInteger(0L);
        var fav = Currencies.GuildGold * 0;

        if (guildRepository.TryGetGuildParticipant(new(address), out var guildParticipant) is true)
        {
            var guild = guildRepository.GetGuild(guildParticipant.GuildAddress);
            var delegatee = guildRepository.GetDelegatee(guild.ValidatorAddress);
            var bond = guildRepository.GetBond(delegatee, guildParticipant.Address);
            var totalDelegated = delegatee.Metadata.TotalDelegatedFAV;
            var totalShare = delegatee.Metadata.TotalShares;
            lastDistributeHeight = bond.LastDistributeHeight ?? -1;
            share = bond.Share;
            fav = (share * totalDelegated).DivRem(totalShare).Quotient;
        }

        return new DelegatorInfo
        {
            LastDistributeHeight = lastDistributeHeight,
            Share = BigIntegerUtility.ToString(share),
            FAV = currencies.ToString(fav),
        };
    }

    public StakeInfo GetStakeInfo(Address address)
    {
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var stakeStateAddress = StakeState.DeriveAddress(address);

        if (world.TryGetStakeState(address, out var stakeState) is false)
        {
            stakeState = default;
        }

        var currency = currencies["ncg"];
        var deposit = world.GetBalance(stakeStateAddress, currency);
        var guildGold = world.GetBalance(stakeStateAddress, Currencies.GuildGold);
        var claimableBlockIndex = stakeState.Contract != null ? stakeState.ClaimableBlockIndex : 0;
        var startedBlockIndex = stakeState.Contract != null ? stakeState.StartedBlockIndex : 0;
        return new StakeInfo
        {
            ClaimableBlockIndex = claimableBlockIndex,
            Deposit = currencies.ToString(deposit),
            StartedBlockIndex = startedBlockIndex,
            GuildGold = currencies.ToString(guildGold),
        };
    }

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var value = await blockChain.GetStateAsync(
            -1, ReservedAddresses.LegacyAccount, GoldCurrencyState.Address, cancellationToken);

        if (value is Dictionary dictionary)
        {
            _goldCurrency = new GoldCurrencyState(dictionary).Currency;
        }
        else
        {
            throw new InvalidOperationException(
                "The states doesn't contain gold currency.\nCheck the genesis block.");
        }
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
