using System.ComponentModel.Composition;
using LibplanetConsole.Common;
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

    string INodeContent.Name => "staking";

    private IStakingService Service => _stakingService.Service;

    Task<ValidatorInfo> IStakingContent.PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
        => Service.PromoteAsync(promoteOptions.Sign(node), cancellationToken);

    Task<ValidatorInfo> IStakingContent.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
        => Service.DelegateAsync(delegateOptions.Sign(node), cancellationToken);

    Task<ValidatorInfo> IStakingContent.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
        => Service.UndelegateAsync(undelegateOptions.Sign(node), cancellationToken);

    Task<ValidatorInfo> IStakingContent.RedelegateAsync(
        RedelegateOptions redelegateOptions, CancellationToken cancellationToken)
        => Service.RedelegateAsync(redelegateOptions.Sign(node), cancellationToken);

    Task<ValidatorInfo[]> IStakingContent.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        AppAddress nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
