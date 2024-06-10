using LibplanetConsole.Common;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Consoles.Stakings;

internal interface IStakingContent
{
    Task<ValidatorInfo> PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken);

    Task<ValidatorInfo> DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken);

    Task<ValidatorInfo> UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken);

    Task<ValidatorInfo> RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken);

    Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<ValidatorInfo> GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken);
}
