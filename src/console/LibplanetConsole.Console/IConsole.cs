namespace LibplanetConsole.Console;

public interface IConsole
{
    const string Tag = "console";

    Address Address { get; }

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
