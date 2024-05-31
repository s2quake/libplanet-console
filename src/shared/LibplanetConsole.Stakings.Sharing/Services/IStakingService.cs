using Libplanet.Crypto;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Stakings.Services;

public interface IStakingService
{
    Task<ValidatorInfo> PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken);

    Task<ValidatorInfo> DelegateAsync(
        DelegateOptions delegateOptions,
        CancellationToken cancellationToken);

    Task<ValidatorInfo> UndelegateAsync(
        UndelegateOptions undelegateOptions,
        CancellationToken cancellationToken);

    Task<ValidatorInfo> RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken);

    Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<ValidatorInfo> GetValidatorAsync(Address nodeAddress, CancellationToken cancellationToken);
}
