namespace LibplanetConsole.Evidence.Services;

public interface IEvidenceService
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken);

    Task ViolateAsync(CancellationToken cancellationToken);

#if LIBPLANET_DPOS
    Task UnjailAsync(byte[] signarue, CancellationToken cancellationToken);
#endif // LIBPLANET_DPOS
}
