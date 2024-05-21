using Libplanet.Crypto;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Clients.Stakings;

internal interface IStakingClient
{
    Task<ValidatorInfo> DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken);

    Task<ValidatorInfo> UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken);

    Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<ValidatorInfo> GetValidatorAsync(Address nodeAddress, CancellationToken cancellationToken);
}
