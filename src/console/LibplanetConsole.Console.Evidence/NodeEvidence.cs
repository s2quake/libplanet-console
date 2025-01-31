using Grpc.Core;
using Libplanet.Types.Evidence;
using LibplanetConsole.Console.Services;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Console.Evidence;

internal sealed class NodeEvidence([FromKeyedServices(INode.Key)] INode node)
    : GrpcNodeContentBase<EvidenceGrpcService.EvidenceGrpcServiceClient>(node, "node-evidence"),
    INodeEvidence
{
    public async Task<EvidenceId> AddEvidenceAsync(CancellationToken cancellationToken)
    {
        var request = new AddEvidenceRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.AddEvidenceAsync(request, callOptions);
        return ToEvidenceId(response.EvidenceId);
    }

    public async Task<EvidenceInfo[]> GetEvidenceAsync(
        long height, CancellationToken cancellationToken)
    {
        var request = new GetEvidenceRequest { Height = height };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetEvidenceAsync(request, callOptions);
        return [.. response.EvidenceInfos.Select(item => (EvidenceInfo)item)];
    }

    public async Task<EvidenceInfo> GetEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        var request = new GetEvidenceRequest { EvidenceId = ToGrpc(evidenceId) };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetEvidenceAsync(request, callOptions);
        return (EvidenceInfo)response.EvidenceInfos[0];
    }

    public async Task<EvidenceInfo[]> GetPendingEvidenceAsync(CancellationToken cancellationToken)
    {
        var request = new GetPendingEvidenceRequest();
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetPendingEvidenceAsync(request, callOptions);
        return [.. response.EvidenceInfos.Select(item => (EvidenceInfo)item)];
    }

    public async Task<EvidenceInfo> GetPendingEvidenceAsync(
        EvidenceId evidenceId, CancellationToken cancellationToken)
    {
        var request = new GetPendingEvidenceRequest
        {
            EvidenceId = ToGrpc(evidenceId),
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        var response = await Service.GetPendingEvidenceAsync(request, callOptions);
        return (EvidenceInfo)response.EvidenceInfos.Single();
    }

    public async Task ViolateAsync(string type, CancellationToken cancellationToken)
    {
        var request = new ViolateRequest
        {
            Type = type,
        };
        var callOptions = new CallOptions(cancellationToken: cancellationToken);
        await Service.ViolateAsync(request, callOptions);
    }
}
