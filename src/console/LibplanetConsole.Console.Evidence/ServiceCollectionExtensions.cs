using JSSoft.Commands;
using LibplanetConsole.Console.Evidence.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Evidence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvidence(this IServiceCollection @this)
    {
        @this.AddKeyedScoped<Evidence>(INode.Key)
             .AddKeyedScoped<IEvidence>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<Evidence>(k))
             .AddKeyedScoped<INodeContent>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<Evidence>(k));

        @this.AddSingleton<ICommand, EvidenceCommand>();

        return @this;
    }
}
