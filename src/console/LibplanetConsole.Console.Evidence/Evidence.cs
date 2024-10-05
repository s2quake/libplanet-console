using LibplanetConsole.Evidence;

namespace LibplanetConsole.Console.Evidence;

internal sealed class Evidence(INode node)
    : INodeContent, IEvidence
    // , INodeContentService
{
    // private readonly RemoteService<IEvidenceService> _evidenceService = new();

    INode INodeContent.Node => node;

    string INodeContent.Name => "evidence";

    // IRemoteService INodeContentService.RemoteService => _evidenceService;

    // private IEvidenceService Service => _evidenceService.Service;

    public Task<EvidenceInfo> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        // return Service.AddEvidenceAsync(cancellationToken);
        throw new NotImplementedException();
    }

    public Task<EvidenceInfo[]> GetEvidenceAsync(long height, CancellationToken cancellationToken)
    {
        // return Service.GetEvidenceAsync(height, cancellationToken);
        throw new NotImplementedException();
    }

    public Task ViolateAsync(CancellationToken cancellationToken)
    {
        // return Service.ViolateAsync(cancellationToken);
        throw new NotImplementedException();
    }

#if LIBPLANET_DPOS
    public Task UnjailAsync(CancellationToken cancellationToken)
    {
        var signature = node.Sign(true);
        return Service.UnjailAsync(signature, cancellationToken);
    }
#endif // LIBPLANET_DPOS
}
