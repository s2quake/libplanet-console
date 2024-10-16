using LibplanetConsole.Console.Services;
using LibplanetConsole.Delegation;
using LibplanetConsole.Delegation.Services;

namespace LibplanetConsole.Console.Delegation;

internal sealed class DelegationNode(INode node)
    : NodeContentBase("delegation-node"), IDelegation
{
    private IDelegationService Service => throw new NotImplementedException();

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

    protected override Task OnStartAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected override Task OnStopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
