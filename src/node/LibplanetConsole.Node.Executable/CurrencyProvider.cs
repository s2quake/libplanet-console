using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Executable;

internal sealed class CurrencyProvider : ICurrencyProvider
{
    public static CurrencyProvider Default { get; } = new();

    public string Code => "won";

    public Currency Currency => Currency.Uncapped("KRW", 2, null);
}
