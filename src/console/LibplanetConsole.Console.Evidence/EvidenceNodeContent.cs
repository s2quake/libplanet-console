using System.ComponentModel.Composition;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Consoles.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Services;

namespace LibplanetConsole.Consoles.Evidence;

[Export(typeof(INodeContent))]
[Export(typeof(IEvidenceContent))]
[Export(typeof(INodeContentService))]
[method: ImportingConstructor]
internal sealed class EvidenceNodeContent(INode node)
    : INodeContent, IEvidenceContent, INodeContentService
{
    private readonly RemoteService<IEvidenceService> _evidenceService = new();

    INode INodeContent.Node => node;

    string INodeContent.Name => "evidence";

    IRemoteService INodeContentService.RemoteService => _evidenceService;

    private IEvidenceService Service => _evidenceService.Service;

    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
        => Service.AddEvidenceAsync(cancellationToken);

    public Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken)
        => Service.GetEvidenceAsync(height, cancellationToken);

    public Task ViolateAsync(CancellationToken cancellationToken)
        => Service.ViolateAsync(cancellationToken);

#if LIBPLANET_DPOS
    public Task UnjailAsync(CancellationToken cancellationToken)
    {
        var signature = node.Sign(true);
        return Service.UnjailAsync(signature, cancellationToken);
    }
#endif // LIBPLANET_DPOS
}
