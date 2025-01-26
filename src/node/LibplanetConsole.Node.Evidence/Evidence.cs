using Libplanet.Blockchain;
using Libplanet.Types.Evidence;

namespace LibplanetConsole.Node.Evidence;

internal sealed class Evidence(INode node)
    : NodeContentBase("evidence"), IEvidence, IAsyncDisposable
{
    private readonly DuplicateVoteExecutor _duplicateVoteExecutor = new(node);

    public Task<EvidenceId> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        var height = blockChain.Tip.Index;
        var validatorAddress = node.Address;
        var evidence = new TestEvidence(height, validatorAddress, DateTimeOffset.UtcNow);
        blockChain.AddEvidence(evidence);
        return Task.FromResult(evidence.Id);
    }

    public async Task<EvidenceInfo[]> GetEvidenceAsync(
        long height, CancellationToken cancellationToken)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        var block = height == -1 ? blockChain.Tip : blockChain[height];
        var evidences = block.Evidence.Select(item => new EvidenceInfo()
        {
            Type = item.GetType().Name,
            Id = item.Id.ToString(),
            Height = item.Height,
            TargetAddress = item.TargetAddress,
            Timestamp = item.Timestamp,
        });
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo> GetEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        if (blockChain.GetCommittedEvidence(evidenceId) is { } evidence)
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
        var blockChain = node.GetRequiredService<BlockChain>();
        var evidences = blockChain.GetPendingEvidence().Select(item => (EvidenceInfo)item);
        await Task.CompletedTask;
        return [.. evidences];
    }

    public async Task<EvidenceInfo> GetPendingEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        var blockChain = node.GetRequiredService<BlockChain>();
        if (blockChain.GetPendingEvidence(evidenceId) is { } evidence)
        {
            await Task.CompletedTask;
            return (EvidenceInfo)evidence;
        }

        throw new ArgumentException(
            message: $"The evidence {evidenceId} does not exist.",
            paramName: nameof(evidenceId));
    }

    public async Task ViolateAsync(string type, CancellationToken cancellationToken)
    {
        if (type == "duplicate_vote")
        {
            await _duplicateVoteExecutor.ExecuteAsync(cancellationToken);
        }
        else
        {
            throw new ArgumentException(
                message: $"The violation type {type} is not supported.",
                paramName: nameof(type));
        }
    }

    public async ValueTask DisposeAsync() => await _duplicateVoteExecutor.DisposeAsync();
}
