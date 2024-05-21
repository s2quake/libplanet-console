using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Consoles.Stakings;

[Export(typeof(INodeContent))]
[Export(typeof(INodeContentService))]
[Export(typeof(IStakingContent))]
[method: ImportingConstructor]
internal sealed class StakingNodeContent(INode node)
    : INodeContent, IStakingContent, INodeContentService
{
    private readonly RemoteService<IStakingService> _stakingService = new();

    IRemoteService INodeContentService.RemoteService => _stakingService;

    INode INodeContent.Node => node;

    private IStakingService Service => _stakingService.Service;

    Task<ValidatorInfo> IStakingContent.PromoteAsync(
        double amount, CancellationToken cancellationToken)
    {
        var signature = node.Sign(amount);
        return Service.PromoteAsync(signature, amount, cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        var signature = node.Sign(amount);
        return Service.DelegateAsync(signature, nodeAddress, amount, cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        Address nodeAddress, long shareAmount, CancellationToken cancellationToken)
    {
        var signature = node.Sign(shareAmount);
        return Service.UndelegateAsync(signature, nodeAddress, shareAmount, cancellationToken);
    }

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
