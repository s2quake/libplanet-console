using LibplanetConsole.Client.Delegation.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Client.Delegation;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseDelegation(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<DelegationServiceGrpcV1>();

        return @this;
    }
}
