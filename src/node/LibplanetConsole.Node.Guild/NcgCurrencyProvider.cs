using Libplanet.Action.State;
using LibplanetConsole.Node.Bank;
using Nekoyume.Model.State;

namespace LibplanetConsole.Node.Guild;

internal sealed class NcgCurrencyProvider(IBlockChain blockChain)
    : ICurrencyProvider
{
    private Currency? _currency;

    public Currency Currency => _currency ??= GetCurrency();

    public string Code => "ncg";

    private Currency GetCurrency()
    {
        var worldStae = blockChain.GetWorldState();
        var accountState = worldStae.GetAccountState(ReservedAddresses.LegacyAccount);
        var value = accountState.GetState(GoldCurrencyState.Address);
        if (value is Dictionary dictionary)
        {
            _currency = new GoldCurrencyState(dictionary).Currency;
            return _currency.Value;
        }

        throw new InvalidOperationException(
            "The states doesn't contain gold currency.\n" +
            "Check the genesis block.");
    }
}
