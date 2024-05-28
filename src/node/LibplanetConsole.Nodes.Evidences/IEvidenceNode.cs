using LibplanetConsole.Evidences.Serializations;

namespace LibplanetConsole.Nodes.Evidences;

public interface IEvidenceNode
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidencesAsync(long height, CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetPendingEvidencesAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo> GetEvidenceAsync(string evidenceId, CancellationToken cancellationToken);

    Task<EvidenceInfo> GetPendingEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken);

    Task UnjailAsync(CancellationToken cancellationToken);
}
