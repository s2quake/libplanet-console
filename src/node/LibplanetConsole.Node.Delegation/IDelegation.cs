using LibplanetConsole.Delegation;

namespace LibplanetConsole.Node.Delegation;

public interface IDelegation
{
    Task<DelegateeInfo> PromoteAsync(
        double amount, CancellationToken cancellationToken);

    Task ClaimAsync(Address nodeAddress, CancellationToken cancellationToken);

    Task<BondInfo> DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken);

    Task<DelegateeInfo> UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken);

    Task<DelegateeInfo> RedelegateAsync(
        Address srcNodeAddress,
        Address destNodeAddress,
        long shareAmount,
        CancellationToken cancellationToken);

    Task<DelegateeInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<DelegateeInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken);

    Task<FungibleAssetValue> GetRewardPoolAsync(
        Address nodeAddress, CancellationToken cancellationToken);
}
