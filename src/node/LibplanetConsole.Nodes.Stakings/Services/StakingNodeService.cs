using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Nodes.Stakings.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class StakingNodeService(StakingNode stakingNode, INode node)
    : LocalService<IStakingService>, IStakingService
{
    public Task<ValidatorInfo> PromoteAsync(
        PromoteOptions promoteOptions, CancellationToken cancellationToken)
    {
        promoteOptions.Verify(node);
        return stakingNode.PromoteAsync(promoteOptions.Amount, cancellationToken);
    }

    public Task<ValidatorInfo> DelegateAsync(
        DelegateOptions delegateOptions, CancellationToken cancellationToken)
    {
        delegateOptions.Verify(node);
        return stakingNode.DelegateAsync(
            nodeAddress: delegateOptions.NodeAddress,
            amount: delegateOptions.Amount,
            cancellationToken: cancellationToken);
    }

    public Task<ValidatorInfo> UndelegateAsync(
        UndelegateOptions undelegateOptions,
        CancellationToken cancellationToken)
    {
        undelegateOptions.Verify(node);
        return stakingNode.UndelegateAsync(
            nodeAddress: undelegateOptions.NodeAddress,
            shareAmount: undelegateOptions.ShareAmount,
            cancellationToken: cancellationToken);
    }

    public Task<ValidatorInfo> RedelegateAsync(
        RedelegateOptions redelegateOptions,
        CancellationToken cancellationToken)
    {
        redelegateOptions.Verify(node);
        return stakingNode.RedelegateAsync(
            srcNodeAddress: redelegateOptions.SrcNodeAddress,
            destNodeAddress: redelegateOptions.DestNodeAddress,
            shareAmount: redelegateOptions.ShareAmount,
            cancellationToken: cancellationToken);
    }

    public Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
        => stakingNode.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => stakingNode.GetValidatorAsync(nodeAddress, cancellationToken);
}
