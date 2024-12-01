namespace LibplanetConsole.Console;

public interface IConsole
{
    Address Address { get; }

    Task<TxId> SendTransactionAsync(IAction[] actions, CancellationToken cancellationToken);
}
