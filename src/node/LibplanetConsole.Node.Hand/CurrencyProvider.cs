using LibplanetConsole.Bank;
using LibplanetConsole.Node.Bank;

namespace LibplanetConsole.Node.Hand;

internal sealed class CurrencyProvider(IApplicationOptions options) : ICurrencyProvider
{
    public IEnumerable<CurrencyInfo> Currencies
    {
        get
        {
            if (Type.GetType(options.ActionProviderType) == typeof(ActionProvider))
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
