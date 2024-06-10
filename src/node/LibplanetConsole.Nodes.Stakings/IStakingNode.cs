using LibplanetConsole.Common;
using LibplanetConsole.Stakings.Serializations;

namespace LibplanetConsole.Nodes.Stakings;

public interface IStakingNode
{
    Task<ValidatorInfo> PromoteAsync(
        double amount, CancellationToken cancellationToken);

    Task<ValidatorInfo> DelegateAsync(
        AppAddress nodeAddress, double amount, CancellationToken cancellationToken);

    Task<ValidatorInfo> UndelegateAsync(
        AppAddress nodeAddress, long shareAmount, CancellationToken cancellationToken);

    Task<ValidatorInfo> RedelegateAsync(
        AppAddress srcNodeAddress,
        AppAddress destNodeAddress,
        long shareAmount,
        CancellationToken cancellationToken);

    Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken);

    Task<ValidatorInfo> GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken);
}
