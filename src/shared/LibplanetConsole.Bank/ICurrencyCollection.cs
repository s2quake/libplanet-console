using System.Diagnostics.CodeAnalysis;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Bank;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Bank;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Bank;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public interface ICurrencyCollection : IEnumerable<Currency>
{
    string[] Codes { get; }

    int Count { get; }

    Currency this[int index] { get; }

    Currency this[string code] { get; }

    bool Contains(string code);

    bool TryGetCurrency(string code, [MaybeNullWhen(false)] out Currency currency);

    string GetCode(Currency currency);

    CurrencyInfo[] GetCurrencyInfos()
        => [.. Codes.Select(item => new CurrencyInfo { Code = item, Currency = this[item] })];

    FungibleAssetValue ToFungibleAssetValue(string amount);

    string ToString(FungibleAssetValue fav);
}
