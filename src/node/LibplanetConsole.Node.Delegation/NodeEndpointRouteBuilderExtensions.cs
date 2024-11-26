using LibplanetConsole.Node.Delegation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node.Delegation;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseDelegation(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<DelegationServiceGrpcV1>();

        return @this;
    }
}
