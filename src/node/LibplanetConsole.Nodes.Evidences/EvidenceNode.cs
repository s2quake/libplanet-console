using System.ComponentModel.Composition;
using Libplanet.Action;
using Libplanet.Types.Evidences;
using LibplanetConsole.Common;
using LibplanetConsole.Evidences;
using LibplanetConsole.Evidences.Serializations;

namespace LibplanetConsole.Nodes.Evidences;

[Export(typeof(IEvidenceNode))]
[Export]
[method: ImportingConstructor]
internal sealed class EvidenceNode(INode node) : IEvidenceNode
{
    public async Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.BlockChain ??
            throw new InvalidOperationException("The node is not running.");
        var height = blockChain.Tip.Index;
        var validatorAddress = node.Address;
        var evidence = new TestEvidence(height, validatorAddress, DateTimeOffset.UtcNow);
        blockChain.AddEvidence(evidence);
        await Task.CompletedTask;
        return (EvidenceInfo)evidence;
    }

    public async Task<EvidenceInfo[]> GetEvidencesAsync(
        long height, CancellationToken cancellationToken)
    {
        var blockChain = node.BlockChain ??
            throw new InvalidOperationException("The node is not running.");
        var block = height == -1 ? blockChain.Tip : blockChain[height];
        var evidences = block.Evidences.Select(item => new EvidenceInfo()
        {
            Type = item.GetType().Name,
            Id = item.Id.ToString(),
            Height = item.Height,
            TargetAddress = AddressUtility.ToString(item.TargetAddress),
            Timestamp = item.Timestamp,
        });
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo[]> GetPendingEvidencesAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.BlockChain ??
                   throw new InvalidOperationException("The node is not running.");
        var evidences = blockChain.GetPendingEvidences().Select(item => (EvidenceInfo)item);
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo> GetEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.BlockChain ??
            throw new InvalidOperationException("The node is not running.");

        if (blockChain.GetCommittedEvidence(EvidenceId.Parse(evidenceId)) is { } evidence)
        {
            await Task.CompletedTask;
            return (EvidenceInfo)evidence;
        }

        throw new ArgumentException(
            message: $"The evidence {evidenceId} does not exist.",
            paramName: nameof(evidenceId));
    }

    public async Task<EvidenceInfo> GetPendingEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.BlockChain ??
            throw new InvalidOperationException("The node is not running.");

        if (blockChain.GetPendingEvidence(EvidenceId.Parse(evidenceId)) is { } evidence)
        {
            await Task.CompletedTask;
            return (EvidenceInfo)evidence;
        }

        throw new ArgumentException(
            message: $"The evidence {evidenceId} does not exist.",
            paramName: nameof(evidenceId));
    }

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
}
