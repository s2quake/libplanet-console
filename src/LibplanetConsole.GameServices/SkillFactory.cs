using LibplanetConsole.GameServices.Serializations;
using LibplanetConsole.GameServices.Skills;

namespace LibplanetConsole.GameServices;

public static class SkillFactory
{
    public static SkillBase Create(Character character, SkillInfo skillInfo)
    {
        return new AttackSkill(character, skillInfo);
    }
}
