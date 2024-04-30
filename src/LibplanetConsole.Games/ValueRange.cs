using Bencodex.Types;
using Newtonsoft.Json;

namespace LibplanetConsole.Games;

public readonly struct ValueRange(long begin, long length) : IEquatable<ValueRange>
{
    public static readonly ValueRange Empty = default;

    public ValueRange(long begin)
        : this(begin, 0)
    {
    }

    public long Begin { get; } = begin;

    public long Length { get; } = length;

    [JsonIgnore]
    public long End { get; } = checked((long)(begin + length));

    [JsonIgnore]
    public readonly long Min => Math.Min(Begin, End);

    [JsonIgnore]
    public readonly long Max => Math.Max(Begin, End);

    [JsonIgnore]
    public readonly long AbsoluteLength => Math.Abs(Length);

    public static bool operator ==(ValueRange range1, ValueRange range2)
    {
        return range1.Begin == range2.Begin && range1.Length == range2.Length;
    }

    public static bool operator !=(ValueRange range1, ValueRange range2)
    {
        return range1.Begin != range2.Begin || range1.Length != range2.Length;
    }

    public static ValueRange FromBencodex(IValue value)
    {
        if (value is not Dictionary values)
        {
            throw new ArgumentException(
                message: $"'{value}' must be a '{typeof(Dictionary)}'",
                paramName: nameof(value));
        }

        return new((Integer)values[nameof(Begin)], (Integer)values[nameof(Length)]);
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

    public long Get(Random random) => random.NextInt64(Begin, End);

    public IValue ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Begin), (Integer)Begin)
                               .Add(nameof(Length), (Integer)Length);
    }

    readonly bool IEquatable<ValueRange>.Equals(ValueRange other)
    {
        return Begin == other.Begin && Length == other.Length;
    }
}
