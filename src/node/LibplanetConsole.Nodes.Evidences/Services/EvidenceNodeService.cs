using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Evidences.Serializations;
using LibplanetConsole.Evidences.Services;

namespace LibplanetConsole.Nodes.Evidences.Services;

[Export(typeof(ILocalService))]
[method: ImportingConstructor]
internal sealed class EvidenceNodeService(EvidenceNode evidenceNode, INode node)
    : LocalService<IEvidenceService>, IEvidenceService
{
    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
        => evidenceNode.AddEvidenceAsync(cancellationToken);

    public Task<EvidenceInfo[]> GetEvidencesAsync(long height, CancellationToken cancellationToken)
        => evidenceNode.GetEvidencesAsync(height, cancellationToken);

    public async Task UnjailAsync(byte[] signarue, CancellationToken cancellationToken)
    {
        if (node.Verify(true, signarue) == true)
        {
            await evidenceNode.UnjailAsync(cancellationToken);
        }

        throw new ArgumentException("Invalid signature.", nameof(signarue));
    }
}
