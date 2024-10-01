using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Client.Delegation;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDelegation(this IServiceCollection @this)
    {
        
        return @this;
    }
}
