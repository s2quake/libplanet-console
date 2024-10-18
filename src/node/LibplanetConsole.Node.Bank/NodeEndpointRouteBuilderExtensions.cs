using LibplanetConsole.Node.Bank.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace LibplanetConsole.Node.Bank;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder UseBank(this IEndpointRouteBuilder @this)
    {
        @this.MapGrpcService<BankServiceGrpcV1>();

        return @this;
    }
}
