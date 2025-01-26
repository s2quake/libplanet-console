using Grpc.Core;
using LibplanetConsole.Grpc.Evidence;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Node.Evidence.Services;

internal sealed class EvidenceServiceGrpcV1(IEvidence evidence)
    : EvidenceGrpcService.EvidenceGrpcServiceBase
{
    public override async Task<AddEvidenceResponse> AddEvidence(
        AddEvidenceRequest request, ServerCallContext context)
    {
        var evidenceId = await evidence.AddEvidenceAsync(context.CancellationToken);
        return new AddEvidenceResponse
        {
            EvidenceId = ToGrpc(evidenceId),
        };
    }

    public override async Task<GetEvidenceResponse> GetEvidence(
        GetEvidenceRequest request, ServerCallContext context)
    {
        if (request.RequestCase == GetEvidenceRequest.RequestOneofCase.Height)
        {
            var height = request.Height;
            var evidenceInfos = await evidence.GetEvidenceAsync(height, context.CancellationToken);
            return new GetEvidenceResponse
            {
                EvidenceInfos = { evidenceInfos.Select(item => (EvidenceInfoProto)item) },
            };
        }
        else if (request.RequestCase == GetEvidenceRequest.RequestOneofCase.EvidenceId)
        {
            var evidenceId = ToEvidenceId(request.EvidenceId);
            var evidenceInfo = await evidence.GetEvidenceAsync(
                evidenceId, context.CancellationToken);
            var evidenceInfos = new EvidenceInfo[] { evidenceInfo };
            return new GetEvidenceResponse
            {
                EvidenceInfos = { evidenceInfos.Select(item => (EvidenceInfoProto)item) },
            };
        }
        else
        {
            throw new InvalidOperationException("Invalid request.");
        }
    }

    public override async Task<GetPendingEvidenceResponse> GetPendingEvidence(
        GetPendingEvidenceRequest request, ServerCallContext context)
    {
        var evidenceInfos = await evidence.GetPendingEvidenceAsync(context.CancellationToken);
        return new GetPendingEvidenceResponse
        {
            EvidenceInfos = { evidenceInfos.Select(item => (EvidenceInfoProto)item) },
        };
    }

    public override async Task<ViolateResponse> Violate(
        ViolateRequest request, ServerCallContext context)
    {
        var type = request.Type;
        await evidence.ViolateAsync(type, context.CancellationToken);
        return new ViolateResponse();
    }
}
