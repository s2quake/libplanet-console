using LibplanetConsole.Bank.Grpc;
using static LibplanetConsole.Grpc.TypeUtility;

namespace LibplanetConsole.Bank;

public readonly partial record struct CurrencyInfo
{
    public string Code { get; init; }

    public Currency Currency { get; init; }

    public static implicit operator CurrencyInfo(CurrencyInfoProto currencyInfo) => new()
    {
        Code = currencyInfo.Code,
        Currency = new Currency(ToIValue(currencyInfo.Currency)),
    };

    public static implicit operator CurrencyInfoProto(CurrencyInfo currencyInfo) => new()
    {
        Code = currencyInfo.Code,
        Currency = ToGrpc(currencyInfo.Currency.Serialize()),
    };
}
