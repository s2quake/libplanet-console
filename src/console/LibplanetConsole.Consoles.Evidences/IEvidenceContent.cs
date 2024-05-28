using LibplanetConsole.Evidences.Serializations;

namespace LibplanetConsole.Consoles.Evidences;

internal interface IEvidenceContent
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidencesAsync(long height, CancellationToken cancellationToken);

    Task UnjailAsync(CancellationToken cancellationToken);
}
