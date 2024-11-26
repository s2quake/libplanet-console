using Lib9c;
using Libplanet.Action.State;
using LibplanetConsole.Node.Delegation.Actions;
using Nekoyume;
using Nekoyume.Action;
using Nekoyume.Action.ValidatorDelegation;
using Nekoyume.Model.State;

namespace LibplanetConsole.Node.Delegation;

internal sealed class Delegation(INode node, IBlockChain blockChain)
    : NodeContentBase(nameof(Delegation)), IDelegation
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
