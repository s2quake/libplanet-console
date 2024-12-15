using System.Numerics;

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

    Task<DelegationInfo> GetInfoAsync(CancellationToken cancellationToken);
}
