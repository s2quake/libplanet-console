using LibplanetConsole.Evidence;

namespace LibplanetConsole.Node.Evidence;

public interface IEvidence
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken);

    Task<EvidenceInfo> GetEvidenceAsync(string evidenceId, CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetPendingEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo> GetPendingEvidenceAsync(
        string evidenceId, CancellationToken cancellationToken);

    Task ViolateAsync(CancellationToken cancellationToken);

#if LIBPLANET_DPOS
    Task UnjailAsync(CancellationToken cancellationToken);
#endif // LIBPLANET_DPOS
}
