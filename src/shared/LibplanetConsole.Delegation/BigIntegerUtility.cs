using System.Globalization;
using System.Numerics;

#if LIBPLANET_NODE
namespace LibplanetConsole.Node.Delegation;
#elif LIBPLANET_CLIENT
namespace LibplanetConsole.Client.Delegation;
#elif LIBPLANET_CONSOLE
namespace LibplanetConsole.Console.Delegation;
#else
#error LIBPLANET_NODE, LIBPLANET_CLIENT, or LIBPLANET_CONSOLE must be defined.
#endif

public static class BigIntegerUtility
{
    public static BigInteger Parse(string value)
        => BigInteger.Parse(value, NumberStyles.AllowDecimalPoint);

    public static string ToString(BigInteger value)
        => value.ToString("N0");
}
