using LibplanetConsole.Grpc.Bank;
using static LibplanetConsole.Grpc.TypeUtility;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Bank;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Bank;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Bank;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

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
