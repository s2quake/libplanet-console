using System.Collections.Concurrent;
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
        Life = (Integer)values[nameof(Life)];
        MaxLife = (Integer)values[nameof(MaxLife)];
        Experience = (Integer)values[nameof(Experience)];
        Level = (Integer)values[nameof(Level)];
        Skills = SkillInfo.FromBencodex((List)values[nameof(Skills)]);
        BlockIndex = (Integer)values[nameof(BlockIndex)];
    }

    public PlayerInfo(Player player)
        : base(player)
    {
        Life = player.Life;
        MaxLife = player.Life;
        Experience = player.Experience;
        Level = player.Level;
        Skills = player.Skills.OfType<SkillBase>().Select(item => (SkillInfo)item).ToArray();
    }

    public static readonly PlayerInfo Empty = new();

    public long Experience { get; init; }

    public long Level { get; init; }

    public SkillInfo[] Skills { get; init; } = Array.Empty<SkillInfo>();

    public long BlockIndex { get; set; }

    public static PlayerInfo CreateNew(string name)
    {
        return new PlayerInfo
        {
            Name = name,
            Life = 1000,
            MaxLife = 1000,
            Skills = new SkillInfo[]
            {
                new() { MaxCoolTime = 3L, CoolTime = 0L, Value = new ValueRange(1, 4) },
            },
        };
    }

    public override Dictionary ToBencodex()
    {
        return base.ToBencodex().Add(nameof(Experience), Experience)
                                .Add(nameof(Level), Level)
                                .Add(nameof(Skills), Skills.ToBencodex())
                                .Add(nameof(BlockIndex), BlockIndex);
    }

    public static explicit operator PlayerInfo(Player player)
    {
        return new PlayerInfo(player);
    }
}
