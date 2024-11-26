namespace LibplanetConsole.Node.Bank;

public interface ICurrencyProvider
{
    string Name { get; }

    Currency Currency { get; }
}
