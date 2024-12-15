namespace LibplanetConsole.Console.Delegation;

public interface IDelegation
{
    Task StakeAsync(long ncg, CancellationToken cancellationToken);

    Task<StakeInfo> GetInfoAsync(CancellationToken cancellationToken);
}
