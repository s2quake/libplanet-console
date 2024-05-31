using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
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
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
    {
        return Service.PromoteAsync(promoteOptions.Sign(node), cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
    {
        return Service.DelegateAsync(delegateOptions.Sign(node), cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
    {
        return Service.UndelegateAsync(undelegateOptions.Sign(node), cancellationToken);
    }

    Task<ValidatorInfo> IStakingContent.RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken)
    {
        return Service.RedelegateAsync(redelegateOptions.Sign(node), cancellationToken);
    }

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
