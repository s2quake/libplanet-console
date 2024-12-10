using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Bank;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Bank;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Bank;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public sealed class CurrencyCollection : IEnumerable<Currency>
{
    private readonly CurrencyInfo[] _currencies;
    private readonly OrderedDictionary _currencyByCode;
    private readonly Dictionary<Currency, string> _codeByCurrency;

    public CurrencyCollection()
    {
        _currencies = [];
        _currencyByCode = [];
        _codeByCurrency = [];
    }

    public CurrencyCollection(CurrencyInfo[] currencies)
    {
        _currencies = currencies;
        _currencyByCode = new OrderedDictionary(currencies.Length);
        foreach (var currency in currencies)
        {
            _currencyByCode.Add(currency.Code, currency.Currency);
        }

        _codeByCurrency = currencies.ToDictionary(item => item.Currency, item => item.Code);
        Aliases = currencies.Select(item => item.Code).ToArray();
    }

    public static CurrencyCollection Empty { get; } = new CurrencyCollection();

    public string[] Aliases { get; } = [];

    public Currency this[int index]
        => (Currency)(_currencyByCode[index] ?? throw new UnreachableException("Cannot happen."));

    public Currency this[string code]
    {
        get
        {
            if (_currencyByCode[code] is not { } currency)
            {
                throw new KeyNotFoundException("No such currency.");
            }

            return (Currency)currency;
        }
    }

    public bool Contains(string code) => _currencyByCode.Contains(code);

    public bool TryGetValue(string code, [MaybeNullWhen(false)] out Currency currency)
    {
        if (_currencyByCode[code] is { } value)
        {
            currency = (Currency)value;
            return true;
        }

        currency = default;
        return false;
    }

    public FungibleAssetValue ToFungibleAssetValue(string amount)
    {
        var match = Regex.Match(amount, @"(?<value>\d+(\.\d+)?)(?<key>\w+)");
        if (match.Success is false)
        {
            throw new ArgumentException("Invalid format.");
        }

        var key = match.Groups["key"].Value;
        var value = match.Groups["value"].Value;
        if (_currencyByCode[key] is { } currency)
        {
            return FungibleAssetValue.Parse((Currency)currency, value);
        }

        throw new ArgumentException("Invalid currency.");
    }

    public string ToString(FungibleAssetValue fav)
    {
        var currency = fav.Currency;
        if (_codeByCurrency.TryGetValue(currency, out var code) is false)
        {
            throw new ArgumentException("Invalid currency.");
        }

        return $"{fav.GetQuantityString()} {code}";
    }

    public CurrencyInfo[] GetCurrencyInfos() => _currencies;

    public CurrencyInfo GetCurrencyInfo(string code)
    {
        if (_currencyByCode[code] is not { } currency)
        {
            throw new KeyNotFoundException("No such currency.");
        }

        return new CurrencyInfo
        {
            Code = code,
            Currency = (Currency)currency,
        };
    }

    public string GetCurrencyAliase(Currency currency)
    {
        if (_codeByCurrency.TryGetValue(currency, out var code) is false)
        {
            throw new KeyNotFoundException("Not supported currency.");
        }

        return code;
    }

    IEnumerator<Currency> IEnumerable<Currency>.GetEnumerator()
        => _currencyByCode.Values.OfType<Currency>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _currencyByCode.Values.GetEnumerator();
}
