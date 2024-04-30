using Bencodex.Types;
using Libplanet.Crypto;

namespace LibplanetConsole.ClientServices.Games.Serializations;

public record RankInfo : IComparable
{
    public static readonly PlayerInfo Empty = new();

    public RankInfo()
    {
    }

    public RankInfo(Dictionary values)
    {
        Address = new Address(values[nameof(Address)]);
        Experience = (Integer)values[nameof(Experience)];
        Level = (Integer)values[nameof(Level)];
    }

    public Address Address { get; init; }

    public long Experience { get; init; }

    public long Level { get; init; }

    public Dictionary ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Address), Address.ToByteArray())
                               .Add(nameof(Experience), Experience)
                               .Add(nameof(Level), Level);
    }

    public static RankInfo[] GetRankInfos(List list)
    {
        var items = new RankInfo[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            items[i] = new RankInfo((Dictionary)list[i]);
        }

        return items;
    }

    public int CompareTo(object? obj)
    {
        if (obj is RankInfo rankInfo)
        {
            if (rankInfo.Level == Level)
            {
                return rankInfo.Experience.CompareTo(Experience);
            }

            return rankInfo.Level.CompareTo(Level);
        }

        return -1;
    }
}
