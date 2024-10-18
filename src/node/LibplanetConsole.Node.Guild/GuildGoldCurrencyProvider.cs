using Lib9c;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Guild;

internal sealed class GuildGoldCurrencyProvider : ICurrencyProvider
{
    public string Name => "gg";

    public Currency Currency => Currencies.GuildGold;
}
