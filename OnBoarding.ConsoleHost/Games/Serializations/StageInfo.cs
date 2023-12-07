using Bencodex.Types;
using Libplanet.Crypto;
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
        Address = new Address(values[nameof(Address)]);
        Monsters = MonsterInfo.FromBencodex((List)values[nameof(Monsters)]);
        Player = new PlayerInfo((Dictionary)values[nameof(Player)]);
    }

    public Address Address { get; init; }

    public MonsterInfo[] Monsters { get; init; } = [];

    public PlayerInfo Player { get; init; } = PlayerInfo.Empty;

    public virtual Dictionary ToBencodex()
    {
        var items = Monsters.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
        return Dictionary.Empty.Add(nameof(Address), Address.ToByteArray())
                               .Add(nameof(Monsters), Monsters.ToBencodex())
                               .Add(nameof(Player), Player.ToBencodex());
    }
}
