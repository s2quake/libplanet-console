using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Executable.Extensions;

public static class NodeEndpointRouteBuilderExtensions
{
    public static IApplicationBuilder TryUseDefaultCurrency(this IApplicationBuilder @this)
    {
        var serviceProvider = @this.ApplicationServices;
        var options = @serviceProvider.GetRequiredService<IApplicationOptions>();
        if (options.ActionProviderType == typeof(ActionProvider).AssemblyQualifiedName)
        {
            var bank = serviceProvider.GetRequiredService<IBank>();
            bank.Currencies.Add(ActionProvider.CurrencyCode, ActionProvider.Currency);
        }

        return @this;
    }
}
