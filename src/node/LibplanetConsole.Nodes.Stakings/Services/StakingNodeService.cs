using System.ComponentModel.Composition;
using Libplanet.Crypto;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Nodes;
using LibplanetConsole.Stakings.Serializations;
using LibplanetConsole.Stakings.Services;

namespace LibplanetConsole.Nodes.Stakings.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class StakingNodeService(StakingNode stakingNode, INode node)
    : LocalService<IStakingService>, IStakingService
{
    public Task<ValidatorInfo> PromoteAsync(
        byte[] signature, double amount, CancellationToken cancellationToken)
    {
        if (node.Verify(amount, signature) == true)
        {
            return stakingNode.PromoteAsync(amount, cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signature));
    }

    public Task<ValidatorInfo> DelegateAsync(
        byte[] signature, Address nodeAddress, double amount, CancellationToken cancellationToken)
    {
        if (node.Verify(amount, signature) == true)
        {
            return stakingNode.DelegateAsync(nodeAddress, amount, cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signature));
    }

    public Task<ValidatorInfo> UndelegateAsync(
        byte[] signature,
        Address nodeAddress,
        long shareAmount,
        CancellationToken cancellationToken)
    {
        if (node.Verify(shareAmount, signature) == true)
        {
            return stakingNode.UndelegateAsync(nodeAddress, shareAmount, cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signature));
    }

    public Task<ValidatorInfo[]> GetValidatorsAsync(CancellationToken cancellationToken)
        => stakingNode.GetValidatorsAsync(cancellationToken);

    public Task<ValidatorInfo> GetValidatorAsync(
        Address nodeAddress, CancellationToken cancellationToken)
        => stakingNode.GetValidatorAsync(nodeAddress, cancellationToken);
}
