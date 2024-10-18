using LibplanetConsole.Node.Bank;
using Nekoyume.Module;

namespace LibplanetConsole.Node.Delegation;

internal sealed class CurrencyProvider(IBlockChain blockChain) : ICurrencyProvider
{
    public string Name => "ncg";

    public Currency Currency => blockChain.GetWorldState().GetGoldCurrency();
}
