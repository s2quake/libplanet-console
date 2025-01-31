using LibplanetConsole.Bank;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Executable;

internal sealed class CurrencyProvider(IApplicationOptions options) : ICurrencyProvider
{
    public IEnumerable<CurrencyInfo> Currencies
    {
        get
        {
            if (options.ActionProviderType == typeof(ActionProvider).AssemblyQualifiedName)
            {
                yield return new CurrencyInfo
                {
                    Code = ActionProvider.CurrencyCode,
                    Currency = ActionProvider.Currency,
                };
            }
        }
    }
}
