using JSSoft.Commands;
using LibplanetConsole.Console.Evidence.Commands;

namespace LibplanetConsole.Console.Evidence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvidence(this IServiceCollection @this)
    {
        @this.AddKeyedScoped<NodeEvidence>(INode.Key)
             .AddKeyedScoped<INodeEvidence>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeEvidence>(k))
             .AddKeyedScoped<INodeContent>(
                INode.Key, (s, k) => s.GetRequiredKeyedService<NodeEvidence>(k));

        @this.AddSingleton<ICommand, NodeEvidenceCommand>();

        return @this;
    }
}
