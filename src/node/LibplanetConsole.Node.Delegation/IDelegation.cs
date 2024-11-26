namespace LibplanetConsole.Node.Delegation;

public interface IDelegation
{
    Task StakeAsync(long amount, CancellationToken cancellationToken);

    Task PromoteAsync(long amount, CancellationToken cancellationToken);
}
