namespace OnBoarding.ConsoleHost.Games;

static class RandomUtility
{
    private static readonly Random Random = new();

    public static long GetNext(long maxValue)
    {
        return Random.NextInt64(maxValue);
    }

    public static long GetNext(long minValue, long maxValue)
    {
        return Random.NextInt64(minValue, maxValue);
    }

    public static int GetNext(int maxValue)
    {
        return Random.Next(maxValue);
    }

    public static int GetNext(int minValue, int maxValue)
    {
        return Random.Next(minValue, maxValue);
    }
}
