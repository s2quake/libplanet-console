using LibplanetConsole.Delegation;

namespace LibplanetConsole.Client.Delegation;

internal interface IDelegation
{
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
}
