using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Services;

namespace LibplanetConsole.Console.Delegation;

internal sealed class DelegationNode(INode node)
    : INodeContent, IDelegation, INodeContentService
{
    private readonly RemoteService<IDelegationService> _delegationService = new();

    IRemoteService INodeContentService.RemoteService => _delegationService;

    INode INodeContent.Node => node;

    string INodeContent.Name => "delegation";

    private IDelegationService Service => _delegationService.Service;

    Task<DelegateeInfo> IDelegation.PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
        => Service.PromoteAsync(promoteOptions.Sign(node), cancellationToken);

    Task<BondInfo> IDelegation.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
        => Service.DelegateAsync(delegateOptions.Sign(node), cancellationToken);

    Task<DelegateeInfo> IDelegation.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
        => Service.UndelegateAsync(undelegateOptions.Sign(node), cancellationToken);

    Task<DelegateeInfo> IDelegation.RedelegateAsync(
        RedelegateOptions redelegateOptions, CancellationToken cancellationToken)
        => Service.RedelegateAsync(redelegateOptions.Sign(node), cancellationToken);

    Task<DelegateeInfo[]> IDelegation.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    public Task<DelegateeInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
