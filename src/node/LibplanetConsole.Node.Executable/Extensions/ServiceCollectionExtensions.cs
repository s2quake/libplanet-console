using JSSoft.Commands;
using LibplanetConsole.Node.Bank;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection TryAddCurrencyProvider(this IServiceCollection @this)
    {
        @this.AddSingleton<ICurrencyProvider>(s =>
        {
            var options = s.GetRequiredService<IOptions<ApplicationOptions>>().Value;
            if (Type.GetType(options.ActionProviderType) is { } type
                && type == typeof(ActionProvider))
            {
                return new CurrencyProvider();
            }

            return null!;
        });

        return @this;
    }
}
