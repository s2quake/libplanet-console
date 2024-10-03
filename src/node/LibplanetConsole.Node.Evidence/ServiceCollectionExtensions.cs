using JSSoft.Commands;
using LibplanetConsole.Common.Services;
using LibplanetConsole.Framework.Extensions;
using LibplanetConsole.Node.Evidence.Commands;
using LibplanetConsole.Node.Evidence.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibplanetConsole.Node.Evidence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEvidence(this IServiceCollection @this)
    {
        @this.AddSingletonWithInterface<IEvidence, Evidence>();
        @this.AddSingleton<ILocalService, EvidenceService>();
        @this.AddSingleton<ICommand, EvidenceCommand>();
        return @this;
    }
}
