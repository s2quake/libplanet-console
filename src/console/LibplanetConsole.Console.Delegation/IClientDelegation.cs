namespace LibplanetConsole.Console.Delegation;

public interface IClientDelegation
{
    Task StakeAsync(long ncg, CancellationToken cancellationToken);

    Task<DelegateeInfo> GetDelegateeInfoAsync(Address address, CancellationToken cancellationToken);

    Task<DelegatorInfo> GetDelegatorInfoAsync(Address address, CancellationToken cancellationToken);
}
