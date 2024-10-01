namespace LibplanetConsole.Delegation.Services;

public interface IDelegationService
{
    Task<DelegateeInfo> PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken);

    Task<BondInfo> DelegateAsync(
        DelegateOptions delegateOptions,
        CancellationToken cancellationToken);

    Task<DelegateeInfo> UndelegateAsync(
        UndelegateOptions undelegateOptions,
        CancellationToken cancellationToken);

    Task<DelegateeInfo> RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken);

    Task<DelegateeInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<DelegateeInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken);
}
