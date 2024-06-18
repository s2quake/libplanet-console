namespace LibplanetConsole.Common;

public static class RandomUtility
{
    private static readonly Random Random = new();

    public static long GetNext(long maxValue) => Random.NextInt64(maxValue);

    public static long GetNext(long minValue, long maxValue)
        => Random.NextInt64(minValue, maxValue);

    public static int GetNext(int maxValue) => Random.Next(maxValue);

    public static int GetNext(int minValue, int maxValue) => Random.Next(minValue, maxValue);
}
