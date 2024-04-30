using LibplanetConsole.Games.Serializations;
using LibplanetConsole.Games.Skills;

namespace LibplanetConsole.Games;

public static class SkillFactory
{
    public static SkillBase Create(Character character, SkillInfo skillInfo)
    {
        return new AttackSkill(character, skillInfo);
    }
}
