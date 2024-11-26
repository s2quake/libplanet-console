using Lib9c;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Guild;

internal sealed class MeadCurrencyProvider : ICurrencyProvider
{
    public string Name => "mead";

    public Currency Currency => Currencies.Mead;
}
