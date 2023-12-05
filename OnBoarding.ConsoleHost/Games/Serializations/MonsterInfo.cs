using Bencodex.Types;

namespace OnBoarding.ConsoleHost.Games;

record MonsterInfo : CharacterInfo
{
    public MonsterInfo()
    {
    }

    public MonsterInfo(Dictionary values)
        : base(values)
    {
        Skills = SkillInfo.FromBencodex((List)values[nameof(Skills)]);
    }

    public SkillInfo[] Skills { get; set; } = [];

    public override Dictionary ToBencodex()
    {
        return base.ToBencodex().Add(nameof(Skills), SkillInfo.ToBencodex(Skills));
    }

    public static List ToBencodex(MonsterInfo[] monsterInfos)
    {
        var l = monsterInfos.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
        return l;
    }

    public static MonsterInfo[] FromBencodex(List list)
    {
        var items = new MonsterInfo[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            items[i] = new MonsterInfo((Dictionary)list[i]);
        }
        return items;
    }

    public static MonsterInfo[] Create(int count)
    {
        var items = new MonsterInfo[count];
        for (var i = 0; i < count; i++)
        {
            var item = new MonsterInfo
            {
                Name = $"Monster {i}",
                Life = 10,
                MaxLife = 10,
                Skills =
                [
                    new SkillInfo{ MaxCoolTime = 10, CoolTime = RandomUtility.GetNext(10), Value = new ValueRange(1, 4) },
                ],
            };
            items[i] = item;
        }
        return items;
    }
}
