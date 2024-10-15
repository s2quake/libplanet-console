using LibplanetConsole.Node.Evidence.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node.Evidence;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseEvidence(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<EvidenceServiceGrpcV1>();

        return @this;
    }
}
