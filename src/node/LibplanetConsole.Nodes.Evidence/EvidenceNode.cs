using System.ComponentModel.Composition;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Libplanet.Types.Evidence;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Serializations;

namespace LibplanetConsole.Nodes.Evidence;

[Export(typeof(IEvidenceNode))]
[Export]
[method: ImportingConstructor]
internal sealed class EvidenceNode(INode node) : IEvidenceNode, IAsyncDisposable
{
    private readonly DuplicateVoteViolator _duplicateVotePerpetrator = new(node);

    public async Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        var height = blockChain.Tip.Index;
        var validatorAddress = node.Address;
        var evidence = new TestEvidence(height, (Address)validatorAddress, DateTimeOffset.UtcNow);
        blockChain.AddEvidence(evidence);
        await Task.CompletedTask;
        return (EvidenceInfo)evidence;
    }

    public async Task<EvidenceInfo[]> GetEvidenceAsync(
        long height, CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        var block = height == -1 ? blockChain.Tip : blockChain[height];
        var evidences = block.Evidence.Select(item => new EvidenceInfo()
        {
            Type = item.GetType().Name,
            Id = item.Id.ToString(),
            Height = item.Height,
            TargetAddress = (AppAddress)item.TargetAddress,
            Timestamp = item.Timestamp,
        });
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo> GetEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        if (blockChain.GetCommittedEvidence(EvidenceId.Parse(evidenceId)) is { } evidence)
        {
            await Task.CompletedTask;
            return (EvidenceInfo)evidence;
        }

        throw new ArgumentException(
            message: $"The evidence {evidenceId} does not exist.",
            paramName: nameof(evidenceId));
    }

    public async Task<EvidenceInfo[]> GetPendingEvidenceAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        var evidences = blockChain.GetPendingEvidence().Select(item => (EvidenceInfo)item);
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo> GetPendingEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.GetService<BlockChain>();
        if (blockChain.GetPendingEvidence(EvidenceId.Parse(evidenceId)) is { } evidence)
        {
            await Task.CompletedTask;
            return (EvidenceInfo)evidence;
        }

        throw new ArgumentException(
            message: $"The evidence {evidenceId} does not exist.",
            paramName: nameof(evidenceId));
    }

    public async Task ViolateAsync(CancellationToken cancellationToken)
    {
        await _duplicateVotePerpetrator.ViolateAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        await _duplicateVotePerpetrator.DisposeAsync();
    }

#if LIBPLANET_DPOS
    public async Task UnjailAsync(CancellationToken cancellationToken)
    {
        var nodeAddress = node.Address;
        var validatorAddress = Nekoyume.Action.DPoS.Model.Validator.DeriveAddress(nodeAddress);
        var actions = new IAction[]
        {
            new Nekoyume.Action.DPoS.Unjail
            {
                Validator = validatorAddress,
            },
        };
        await node.AddTransactionAsync(actions, cancellationToken);
    }
#endif //LIBPLANET_DPOS
}
