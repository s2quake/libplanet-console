using System.Numerics;
using Lib9c;
using Libplanet.Action.State;
using LibplanetConsole.Node.Delegation.Actions;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.State;
using Nekoyume.ValidatorDelegation;

namespace LibplanetConsole.Node.Delegation;

internal sealed class Validator(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Validator)), IValidator
{
    private Currency? _goldCurrency;

    public Currency GoldCurrency => _goldCurrency ?? throw new InvalidOperationException(
        "There is no currency information.");

    public async Task StakeAsync(long amount, CancellationToken cancellationToken)
    {
        var stakeAction = new StakeAction(amount);
        await blockChain.SendTransactionAsync([stakeAction], cancellationToken);
    }

    public async Task PromoteAsync(long amount, CancellationToken cancellationToken)
    {
        var publicKey = node.PublicKey;
        var fav = Currencies.GuildGold * amount;
        var promoteValidator = new PromoteValidator(publicKey, fav);
        await blockChain.SendTransactionAsync([promoteValidator], cancellationToken);
    }

    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var unjailValidator = new UnjailValidator();
        await blockChain.SendTransactionAsync([unjailValidator], cancellationToken);
    }

    public async Task DelegateAsync(FungibleAssetValue amount, CancellationToken cancellationToken)
    {
        var delegateAction = new DelegateValidator(amount);
        await node.SendTransactionAsync([delegateAction], cancellationToken);
    }

    public async Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken)
    {
        var undelegateAction = new UndelegateValidator(share);
        await node.SendTransactionAsync([undelegateAction], cancellationToken);
    }

    public Task<ValidatorInfo> GetInfoAsync(CancellationToken cancellationToken)
    {
        var worldState = blockChain.GetWorldState();
        var world = new World(worldState);
        var repository = new ValidatorRepository(world, new DummayActionContext());
        var delegatee = repository.GetValidatorDelegatee(node.Address);
        var info = new ValidatorInfo
        {
            Power = delegatee.Power.ToString("N0"),
            TotalShare = delegatee.TotalShares.ToString("N0"),
            IsJailed = delegatee.Jailed,
            JailedUntil = delegatee.JailedUntil,
        };

        return Task.FromResult(info);
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
                   "The states doesn't contain gold currency.\n" +
                   "Check the genesis block.");
        }
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
