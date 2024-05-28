using LibplanetConsole.Evidences.Serializations;

namespace LibplanetConsole.Evidences.Services;

public interface IEvidenceService
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidencesAsync(long height, CancellationToken cancellationToken);

    Task UnjailAsync(byte[] signarue, CancellationToken cancellationToken);
}
