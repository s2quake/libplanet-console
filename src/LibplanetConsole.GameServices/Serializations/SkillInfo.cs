using Bencodex.Types;

namespace LibplanetConsole.GameServices.Serializations;

public record SkillInfo
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

    public static explicit operator SkillInfo(SkillBase skill)
    {
        return new SkillInfo
        {
            Value = skill.Value,
            CoolTime = skill.CoolTime,
            MaxCoolTime = skill.MaxCoolTime,
        };
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

    public virtual Dictionary ToBencodex()
    {
        return Dictionary.Empty.Add(nameof(Value), Value.ToBencodex())
                               .Add(nameof(CoolTime), CoolTime)
                               .Add(nameof(MaxCoolTime), MaxCoolTime);
    }
}
