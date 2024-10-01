using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Services;

namespace LibplanetConsole.Console.Delegation;

internal sealed class DelegationClient(IClient client)
    : IClientContent, IDelegation, IClientContentService
{
    private readonly RemoteService<IDelegationService> _delegationService = new();

    IRemoteService IClientContentService.RemoteService => _delegationService;

    IClient IClientContent.Client => client;

    string IClientContent.Name => "delegation";

    private IDelegationService Service => _delegationService.Service;

    Task<DelegateeInfo> IDelegation.PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
        => throw new NotSupportedException("Promote is not supported.");

    Task<BondInfo> IDelegation.DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
        => Service.DelegateAsync(delegateOptions.Sign(client), cancellationToken);

    Task<DelegateeInfo> IDelegation.UndelegateAsync(
        UndelegateOptions undelegateOptions, CancellationToken cancellationToken)
        => Service.UndelegateAsync(undelegateOptions.Sign(client), cancellationToken);

    Task<DelegateeInfo> IDelegation.RedelegateAsync(
        RedelegateOptions redelegateOptions, CancellationToken cancellationToken)
        => Service.RedelegateAsync(redelegateOptions.Sign(client), cancellationToken);

    Task<DelegateeInfo[]> IDelegation.GetValidatorsAsync(CancellationToken cancellationToken)
        => Service.GetValidatorsAsync(cancellationToken);

    Task<DelegateeInfo> IDelegation.GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => Service.GetValidatorAsync(nodeAddress, cancellationToken);
}
