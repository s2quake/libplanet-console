namespace LibplanetConsole.Node.Bank;

public interface ICurrencyProvider
{
    string Code { get; }

    Currency Currency { get; }
}
