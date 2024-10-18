using Lib9c;
using LibplanetConsole.Node.Bank;
using Nekoyume.Module;

namespace LibplanetConsole.Node.Guild;

internal sealed class GuildGoldCurrencyProvider(IBlockChain blockChain) : ICurrencyProvider
{
    public string Name => "gg";

    public Currency Currency => Currencies.GuildGold;
}
