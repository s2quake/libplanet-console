using LibplanetConsole.Evidence;

namespace LibplanetConsole.Consoles.Evidence;

internal interface IEvidenceContent
{
    Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken);

    Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken);

    Task ViolateAsync(CancellationToken cancellationToken);

#if LIBPLANET_DPOS
    Task UnjailAsync(CancellationToken cancellationToken);
#endif // LIBPLANET_DPOS
}
