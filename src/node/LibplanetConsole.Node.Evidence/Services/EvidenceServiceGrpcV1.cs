using Grpc.Core;
using LibplanetConsole.Evidence;
using LibplanetConsole.Evidence.Grpc;

namespace LibplanetConsole.Node.Evidence.Services;

internal sealed class EvidenceServiceGrpcV1(Evidence evidence)
    : EvidenceGrpcService.EvidenceGrpcServiceBase
{
    public override async Task<AddEvidenceResponse> AddEvidence(
        AddEvidenceRequest request, ServerCallContext context)
    {
        var evidenceInfo = await evidence.AddEvidenceAsync(context.CancellationToken);
        return new AddEvidenceResponse
        {
            EvidenceInfo = evidenceInfo,
        };
    }

    public override async Task<GetEvidenceResponse> GetEvidence(
        GetEvidenceRequest request, ServerCallContext context)
    {
        var height = request.Height;
        var evidenceInfos = await evidence.GetEvidenceAsync(height, context.CancellationToken);
        var response = new GetEvidenceResponse();
        for (var i = 0; i < evidenceInfos.Length; i++)
        {
            response.EvidenceInfos.Add(evidenceInfos[i]);
        }

        return response;
    }

    public override Task<ViolateResponse> Violate(
        ViolateRequest request, ServerCallContext context)
    {
        return base.Violate(request, context);
    }
}
