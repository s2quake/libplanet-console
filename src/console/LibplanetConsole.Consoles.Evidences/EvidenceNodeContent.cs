using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Evidences.Serializations;
using LibplanetConsole.Evidences.Services;

namespace LibplanetConsole.Consoles.Evidences;

[Export(typeof(INodeContent))]
[Export(typeof(IEvidenceContent))]
[Export(typeof(INodeContentService))]
[method: ImportingConstructor]
internal sealed class EvidenceNodeContent(INode node)
    : INodeContent, IEvidenceContent, INodeContentService
{
    private readonly RemoteService<IEvidenceService> _evidenceService = new();

    INode INodeContent.Node => node;

    IRemoteService INodeContentService.RemoteService => _evidenceService;

    private IEvidenceService Service => _evidenceService.Service;

    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
        => Service.AddEvidenceAsync(cancellationToken);

    public Task<EvidenceInfo[]> GetEvidencesAsync(long height, CancellationToken cancellationToken)
        => Service.GetEvidencesAsync(height, cancellationToken);

    public Task UnjailAsync(CancellationToken cancellationToken)
    {
        var signature = node.Sign(true);
        return Service.UnjailAsync(signature, cancellationToken);
    }
}
