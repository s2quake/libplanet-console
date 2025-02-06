using System.Globalization;
using System.Numerics;

namespace LibplanetConsole.Delegation;

public static class BigIntegerUtility
{
    public static BigInteger Parse(string value)
        => BigInteger.Parse(value, NumberStyles.Number);

    public static string ToString(BigInteger value)
        => value.ToString("N0");
}
