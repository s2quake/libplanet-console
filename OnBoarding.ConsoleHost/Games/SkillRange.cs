namespace OnBoarding.ConsoleHost.Games;

readonly struct SkillRange(long begin, long length) : IEquatable<SkillRange>
{
    private static readonly Random Random = new();

    public SkillRange(long begin)
        : this(begin, 0)
    {
    }

    public static SkillRange FromBeginEnd(long begin, long end)
    {
        return new SkillRange(begin, end - begin);
    }

    public override readonly bool Equals(object? obj)
    {
        if (obj is SkillRange range)
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

    public static readonly SkillRange Empty = new();

    public static bool operator ==(SkillRange range1, SkillRange range2)
    {
        return range1.Begin == range2.Begin && range1.Length == range2.Length;
    }

    public static bool operator !=(SkillRange range1, SkillRange range2)
    {
        return range1.Begin != range2.Begin || range1.Length != range2.Length;
    }

    #region IEquatable

    readonly bool IEquatable<SkillRange>.Equals(SkillRange other)
    {
        return Begin == other.Begin && Length == other.Length;
    }

    #endregion
}