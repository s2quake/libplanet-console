using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text.RegularExpressions;
using LibplanetConsole.Bank.DataAnnotations;
using LibplanetConsole.Common;

namespace LibplanetConsole.Bank;

internal abstract class CurrencyCollectionBase : ICurrencyCollection
{
    private readonly OrderedDictionary _currencyByCode;
    private readonly Dictionary<Currency, string> _codeByCurrency;

    protected CurrencyCollectionBase()
    {
        _currencyByCode = [];
        _codeByCurrency = [];
    }

    protected CurrencyCollectionBase(IEnumerable<KeyValuePair<string, Currency>> currencies)
    {
        _currencyByCode = new OrderedDictionary(currencies.Count());
        foreach (var currency in currencies)
        {
            _currencyByCode.Add(currency.Key, currency.Value);
        }

        _codeByCurrency = currencies.ToDictionary(item => item.Value, item => item.Key);
    }

    public string[] Codes => [.. _codeByCurrency.Values];

    public int Count => _currencyByCode.Count;

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

    public void Add(string code, Currency currency)
    {
        _currencyByCode.Add(code, currency);
        _codeByCurrency.Add(currency, code);
    }

    public bool Contains(string code) => _currencyByCode.Contains(code);

    public bool Remove(string code)
    {
        if (_currencyByCode[code] is not Currency currency)
        {
            return false;
        }

        _currencyByCode.Remove(code);
        _codeByCurrency.Remove(currency);
        return true;
    }

    public void RemoteAt(int index)
    {
        if (_currencyByCode[index] is not Currency currency)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        _currencyByCode.RemoveAt(index);
        _codeByCurrency.Remove(currency);
    }

    public void Clear()
    {
        _currencyByCode.Clear();
        _codeByCurrency.Clear();
    }

    public bool TryGetCurrency(string code, [MaybeNullWhen(false)] out Currency currency)
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
        var match = Regex.Match(amount, $"^{FungibleAssetValueAttribute.RegularExpression}$");
        if (match.Success is false)
        {
            throw new ArgumentException($"Invalid format: {amount}", nameof(amount));
        }

        var code = match.Groups["code"].Value;
        var integer = BigIntegerUtility.Parse(match.Groups["integer"].Value);
        var @decimal = match.Groups["decimal"].Value;
        if (_currencyByCode[code] is { } currency)
        {
            return FungibleAssetValue.Parse((Currency)currency, $"{integer}{@decimal}");
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

        var ss = fav.GetQuantityString().Split('.');
        var s1 = BigInteger.Parse(ss[0]).ToString("N0");
        if (ss.Length > 1)
        {
            return $"{s1}.{ss[1]} {code}";
        }

        return $"{s1} {code}";
    }

    public string GetCode(Currency currency)
    {
        if (_codeByCurrency.TryGetValue(currency, out var code) is false)
        {
            throw new KeyNotFoundException("Not supported currency.");
        }

        return code;
    }

    public CurrencyInfo[] GetCurrencyInfos() => [.. _codeByCurrency.Select(GetAliasInfo)];

    IEnumerator<Currency> IEnumerable<Currency>.GetEnumerator()
        => _currencyByCode.Values.OfType<Currency>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _currencyByCode.Values.GetEnumerator();

    private static CurrencyInfo GetAliasInfo(KeyValuePair<Currency, string> item)
        => new() { Code = item.Value, Currency = item.Key };
}
