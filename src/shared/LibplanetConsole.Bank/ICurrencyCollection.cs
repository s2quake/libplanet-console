using System.Diagnostics.CodeAnalysis;

namespace LibplanetConsole.Bank;

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
