namespace OnBoarding.ConsoleHost.Games;

readonly struct ValueRange(long begin, long length) : IEquatable<ValueRange>
{
    private static readonly Random Random = new();

    public ValueRange(long begin)
        : this(begin, 0)
    {
    }

    public static ValueRange FromBeginEnd(long begin, long end)
    {
        return new ValueRange(begin, end - begin);
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is ValueRange range)
        {
            return Begin == range.Begin && Length == range.Length;
        }
        return base.Equals(obj);
    }

    public override readonly int GetHashCode()
    {
        return Begin.GetHashCode() ^ Length.GetHashCode();
    }

    public override readonly string ToString()
    {
        return $"{Begin}, {Length}";
    }

    public bool Contains(long value) => value >= Begin && value < End;

    public long Get() => Random.NextInt64(Begin, End);

    public long Begin { get; } = begin;

    public long Length { get; } = length;

    public long End { get; } = checked((long)(begin + length));

    public readonly long Min => Math.Min(Begin, End);

    public readonly long Max => Math.Max(Begin, End);

    public readonly long AbsoluteLength => Math.Abs(Length);

    public static readonly ValueRange Empty = new();

    public static bool operator ==(ValueRange range1, ValueRange range2)
    {
        return range1.Begin == range2.Begin && range1.Length == range2.Length;
    }

    public static bool operator !=(ValueRange range1, ValueRange range2)
    {
        return range1.Begin != range2.Begin || range1.Length != range2.Length;
    }

    #region IEquatable

    readonly bool IEquatable<ValueRange>.Equals(ValueRange other)
    {
        return Begin == other.Begin && Length == other.Length;
    }

    #endregion
}