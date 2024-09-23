using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Services;

namespace LibplanetConsole.Nodes.Evidence.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class EvidenceNodeService(EvidenceNode evidenceNode)
    : LocalService<IEvidenceService>, IEvidenceService
{
    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
        => evidenceNode.AddEvidenceAsync(cancellationToken);

    public Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken)
        => evidenceNode.GetEvidenceAsync(height, cancellationToken);

    public Task ViolateAsync(CancellationToken cancellationToken)
        => evidenceNode.ViolateAsync(cancellationToken);

#if LIBPLANET_DPOS
    public async Task UnjailAsync(byte[] signarue, CancellationToken cancellationToken)
    {
        if (node.Verify(true, signarue) == true)
        {
            await evidenceNode.UnjailAsync(cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signarue));
    }
#endif // LIBPLANET_DPOS
}
