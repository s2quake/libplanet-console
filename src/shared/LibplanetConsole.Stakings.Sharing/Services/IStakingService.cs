using Libplanet.Crypto;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Stakings.Services;

public interface IStakingService
{
    Task<ValidatorInfo> PromoteAsync(
        byte[] signature, double amount, CancellationToken cancellationToken);

    Task<ValidatorInfo> DelegateAsync(
        byte[] signature, Address address, double amount, CancellationToken cancellationToken);

    Task<ValidatorInfo> UndelegateAsync(
        byte[] signature, Address address, long shareAmount, CancellationToken cancellationToken);

    Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<ValidatorInfo> GetValidatorAsync(Address nodeAddress, CancellationToken cancellationToken);
}
