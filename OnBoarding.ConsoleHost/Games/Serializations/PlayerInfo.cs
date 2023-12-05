using Bencodex.Types;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

record PlayerInfo : CharacterInfo
{
    public PlayerInfo()
    {
    }

    public PlayerInfo(Dictionary values)
        : base(values)
    {
        Address = new Address(values[nameof(Address)]);
        Life = (Integer)values[nameof(Life)];
        MaxLife = (Integer)values[nameof(MaxLife)];
        Experience = (Integer)values[nameof(Experience)];
        Level = (Integer)values[nameof(Level)];
        Skills = SkillInfo.FromBencodex((List)values[nameof(Skills)]);
    }

    public PlayerInfo(Player player)
        : base(player)
    {
        Address = player.Address;
        Life = player.Life;
        MaxLife = player.Life;
        Experience = player.Experience;
        Level = player.Level;
        Skills = player.Skills.OfType<SkillBase>().Select(item => (SkillInfo)item).ToArray();
    }

    public static readonly PlayerInfo Empty = new();

    public Address Address { get; init; }

    public long Experience { get; init; }

    public long Level { get; init; }

    public SkillInfo[] Skills { get; init; } = [];

    public override Dictionary ToBencodex()
    {
        return base.ToBencodex().Add(nameof(Address), Address.ToByteArray())
                                .Add(nameof(Experience), Experience)
                                .Add(nameof(Level), Level)
                                .Add(nameof(Skills), SkillInfo.ToBencodex(Skills));
    }

    public static explicit operator PlayerInfo(Player player)
    {
        return new PlayerInfo(player);
    }
}
