using System.Numerics;

namespace LibplanetConsole.Node.Delegation;

public interface IValidator
{
    Task StakeAsync(long amount, CancellationToken cancellationToken);

    Task PromoteAsync(long amount, CancellationToken cancellationToken);

    Task UnjailAsync(CancellationToken cancellationToken);

    Task DelegateAsync(FungibleAssetValue amount, CancellationToken cancellationToken);

    Task UndelegateAsync(BigInteger share, CancellationToken cancellationToken);

    Task<ValidatorInfo> GetInfoAsync(CancellationToken cancellationToken);
}
