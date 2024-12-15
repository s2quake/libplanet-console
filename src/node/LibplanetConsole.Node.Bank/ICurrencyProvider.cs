namespace LibplanetConsole.Node.Bank;

public interface ICurrencyProvider
{
    IEnumerable<CurrencyInfo> Currencies { get; }
}
