using Bencodex.Types;

namespace OnBoarding.ConsoleHost.Games;

record SkillInfo
{
    public SkillInfo()
    {
    }

    public SkillInfo(Dictionary values)
    {
        Value = ValueRange.FromBencodex(values[nameof(Value)]);
        CoolTime = (Integer)values[nameof(CoolTime)];
        MaxCoolTime = (Integer)values[nameof(MaxCoolTime)];
    }

    public ValueRange Value { get; set; }

    public long CoolTime { get; set; }

    public long MaxCoolTime { get; set; }

    public virtual Dictionary ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Value), Value.ToBencodex())
                               .Add(nameof(CoolTime), CoolTime)
                               .Add(nameof(MaxCoolTime), MaxCoolTime);
    }

    public static List ToBencodex(SkillInfo[] skillInfos)
    {
        return skillInfos.Aggregate(List.Empty, (l, n) => l.Add(n.ToBencodex()));
    }

    public static SkillInfo[] FromBencodex(List list)
    {
        var items = new SkillInfo[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            items[i] = new SkillInfo((Dictionary)list[i]);
        }
        return items;
    }

    public static explicit operator SkillInfo(SkillBase skill)
    {
        return new SkillInfo
        {
            Value = skill.Value,
            CoolTime = skill.CoolTime,
            MaxCoolTime = skill.MaxCoolTime,
        };
    }
}
