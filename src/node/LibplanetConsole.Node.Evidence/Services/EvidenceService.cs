using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Services;

namespace LibplanetConsole.Node.Evidence.Services;

[Export(typeof(ILocalService))]
internal sealed class EvidenceService(Evidence evidence)
    : LocalService<IEvidenceService>, IEvidenceService
{
    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
        => evidence.AddEvidenceAsync(cancellationToken);

    public Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken)
        => evidence.GetEvidenceAsync(height, cancellationToken);

    public Task ViolateAsync(CancellationToken cancellationToken)
        => evidence.ViolateAsync(cancellationToken);

#if LIBPLANET_DPOS
    public async Task UnjailAsync(byte[] signarue, CancellationToken cancellationToken)
    {
        if (node.Verify(true, signarue) == true)
        {
            await evidence.UnjailAsync(cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signarue));
    }
#endif // LIBPLANET_DPOS
}
