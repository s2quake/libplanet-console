using JSSoft.Commands;
using LibplanetConsole.Node.Evidence.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Evidence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvidence(this IServiceCollection @this)
    {
        @this.AddSingleton<Evidence>()
             .AddSingleton<IEvidence>(s => s.GetRequiredService<Evidence>());
        // @this.AddSingleton<ILocalService, EvidenceService>();
        @this.AddSingleton<ICommand, EvidenceCommand>();
        return @this;
    }
}