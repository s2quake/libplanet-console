using LibplanetConsole.Delegation;

namespace LibplanetConsole.Console.Delegation;

public interface IDelegation
{
    Task StakeAsync(long ncg, CancellationToken cancellationToken);

    Task<DelegateeInfo> GetDelegateeInfoAsync(Address address, CancellationToken cancellationToken);

    Task<DelegatorInfo> GetDelegatorInfoAsync(Address address, CancellationToken cancellationToken);

    Task<StakeInfo> GetStakeInfoAsync(Address address, CancellationToken cancellationToken);
}
