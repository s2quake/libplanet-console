using Lib9c;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Guild;

internal sealed class CurrencyProvider : ICurrencyProvider
{
    private readonly CurrencyInfo[] _currencyInfos =
    [
        new() { Code = "mead", Currency = Currencies.Mead },
        new() { Code = "gg", Currency = Currencies.GuildGold },
    ];

    IEnumerable<CurrencyInfo> ICurrencyProvider.Currencies => _currencyInfos;
}
