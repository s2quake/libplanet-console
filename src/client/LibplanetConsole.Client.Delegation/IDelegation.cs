namespace LibplanetConsole.Client.Delegation;

public interface IDelegation
{
    Task StakeAsync(long ncg, CancellationToken cancellationToken);

    Task<DelegatorInfo> GetDelegatorInfoAsync(Address address, CancellationToken cancellationToken);
}
