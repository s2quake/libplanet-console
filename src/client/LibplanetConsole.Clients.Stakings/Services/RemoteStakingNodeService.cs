using System.ComponentModel.Composition;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Clients.Stakings.Services;

[Export(typeof(IStakingClient))]
[Export(typeof(IRemoteService))]
internal sealed class RemoteStakingNodeService
    : RemoteService<IStakingService>, IStakingClient
{
    public Task<ValidatorInfo> DelegateAsync(
        AppAddress nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var options = new DelegateOptions
        {
            NodeAddress = nodeAddress,
            Amount = amount,
        };
        return Service.DelegateAsync(options, cancellationToken);
    }

    public Task<ValidatorInfo> UndelegateAsync(
        AppAddress nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var options = new UndelegateOptions
        {
            NodeAddress = nodeAddress,
            ShareAmount = shareAmount,
        };
        return Service.UndelegateAsync(options, cancellationToken);
    }

    public async Task<ValidatorInfo> RedelegateAsync(
        AppAddress srcNodeAddress,
        AppAddress destNodeAddress,
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

    public Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
