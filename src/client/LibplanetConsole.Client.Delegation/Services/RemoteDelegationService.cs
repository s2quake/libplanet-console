using LibplanetConsole.Common.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Services;

namespace LibplanetConsole.Client.Delegation.Services;

internal sealed class RemoteDelegationService
    : RemoteService<IDelegationService>, IDelegation
{
    public Task<BondInfo> DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var options = new DelegateOptions
        {
            NodeAddress = nodeAddress,
            Amount = amount,
        };
        return Service.DelegateAsync(options, cancellationToken);
    }

    public Task<DelegateeInfo> UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var options = new UndelegateOptions
        {
            NodeAddress = nodeAddress,
            ShareAmount = shareAmount,
        };
        return Service.UndelegateAsync(options, cancellationToken);
    }

    public async Task<DelegateeInfo> RedelegateAsync(
        Address srcNodeAddress,
        Address destNodeAddress,
        long shareAmount,
        CancellationToken cancellationToken)
    {
        var options = new RedelegateOptions
        {
            SrcNodeAddress = srcNodeAddress,
            DestNodeAddress = destNodeAddress,
            ShareAmount = shareAmount,
        };
        return await Service.RedelegateAsync(options, cancellationToken);
    }

    public Task<DelegateeInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<DelegateeInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
