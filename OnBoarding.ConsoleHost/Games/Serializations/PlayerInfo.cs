using Bencodex.Types;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games.Serializations.Extensions;

namespace OnBoarding.ConsoleHost.Games.Serializations;

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

    public static PlayerInfo CreateNew(string name, Address address)
    {
        return new PlayerInfo
        {
            Name = name,
            Address = address,
            Life = 1000,
            MaxLife = 1000,
            Skills =
            [
                new SkillInfo{ MaxCoolTime = 3L, CoolTime = 0L, Value = new ValueRange(1, 4) },
            ],
        };
    }

    public override Dictionary ToBencodex()
    {
        return base.ToBencodex().Add(nameof(Address), Address.ToByteArray())
                                .Add(nameof(Experience), Experience)
                                .Add(nameof(Level), Level)
                                .Add(nameof(Skills), Skills.ToBencodex());
    }

    public static explicit operator PlayerInfo(Player player)
    {
        return new PlayerInfo(player);
    }
}
