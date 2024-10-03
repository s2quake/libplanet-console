using JSSoft.Commands;
using LibplanetConsole.Console.Evidence.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Console.Evidence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvidence(this IServiceCollection @this)
    {
        @this.AddScoped<Evidence>()
             .AddScoped<IEvidence>(s => s.GetRequiredService<Evidence>());

        @this.AddSingleton<ICommand, EvidenceCommand>();

        return @this;
    }
}
