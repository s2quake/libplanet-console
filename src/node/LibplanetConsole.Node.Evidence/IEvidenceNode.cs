using LibplanetConsole.Evidence;

namespace LibplanetConsole.Nodes.Evidence;

public interface IEvidenceNode
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
