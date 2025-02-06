using System.Numerics;
using LibplanetConsole.Delegation;

namespace LibplanetConsole.Console.Delegation;

public interface INodeDelegation
{
    Task StakeAsync(long ncg, CancellationToken cancellationToken);

    Task PromoteAsync(FungibleAssetValue guildGold, CancellationToken cancellationToken);

    Task UnjailAsync(CancellationToken cancellationToken);

    Task DelegateAsync(FungibleAssetValue guildGold, CancellationToken cancellationToken);

    Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken);

    Task SetCommissionAsync(long commission, CancellationToken cancellationToken);

    Task ClaimAsync(CancellationToken cancellationToken);

    Task SlashAsync(long slashFactor, CancellationToken cancellationToken);

    Task<DelegateeInfo> GetDelegateeInfoAsync(Address address, CancellationToken cancellationToken);

    Task<DelegatorInfo> GetDelegatorInfoAsync(Address address, CancellationToken cancellationToken);

    Task<StakeInfo> GetStakeInfoAsync(Address address, CancellationToken cancellationToken);
}
