using System.Numerics;

namespace LibplanetConsole.Node.Delegation;

public interface IDelegation
{
    Task StakeAsync(FungibleAssetValue ncg, CancellationToken cancellationToken);

    Task PromoteAsync(FungibleAssetValue guildGold, CancellationToken cancellationToken);

    Task UnjailAsync(CancellationToken cancellationToken);

    Task DelegateAsync(FungibleAssetValue guildGold, CancellationToken cancellationToken);

    Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken);

    Task SetCommissionAsync(long commission, CancellationToken cancellationToken);

    Task ClaimAsync(CancellationToken cancellationToken);

    Task<NodeDelegationInfo> GetInfoAsync(CancellationToken cancellationToken);
}
