using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Services;

namespace LibplanetConsole.Console.Evidence;

internal sealed class Evidence(INode node)
    : INodeContent, IEvidence, INodeContentService
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
