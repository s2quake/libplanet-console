using Bencodex.Types;
using OnBoarding.ConsoleHost.Games.Serializations.Extensions;

namespace OnBoarding.ConsoleHost.Games.Serializations;

record StageInfo
{
    public static readonly StageInfo Empty = new();

    public StageInfo()
    {
    }

    public StageInfo(Dictionary values)
    {
        Monsters = MonsterInfo.FromBencodex((List)values[nameof(Monsters)]);
        Player = new PlayerInfo((Dictionary)values[nameof(Player)]);
    }

    public MonsterInfo[] Monsters { get; init; } = [];

    public PlayerInfo Player { get; init; } = PlayerInfo.Empty;

    public virtual Dictionary ToBencodex()
    {
        var items = Monsters.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
        return Dictionary.Empty.Add(nameof(Monsters), Monsters.ToBencodex())
                               .Add(nameof(Player), Player.ToBencodex());
    }
}
