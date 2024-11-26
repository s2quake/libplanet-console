using Libplanet.Action.State;
using LibplanetConsole.Node.Bank;
using Nekoyume.Model.State;

namespace LibplanetConsole.Node.Guild;

internal sealed class NcgCurrencyProvider(IBlockChain blockChain)
    : NodeContentBase("ncg"), ICurrencyProvider
{
    private Currency? _currency;

    public Currency Currency => _currency ?? throw new InvalidOperationException(
        "There is no currency information.");

    protected override async Task OnStartAsync(CancellationToken cancellationToken)
    {
        var value = await blockChain.GetStateAsync(
            -1, ReservedAddresses.LegacyAccount, GoldCurrencyState.Address, cancellationToken);

        if (value is Dictionary dictionary)
        {
            _currency = new GoldCurrencyState(dictionary).Currency;
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
        _currency = null;
        return Task.CompletedTask;
    }
}
