using LibplanetConsole.ClientServices.Games.Serializations;
using LibplanetConsole.ClientServices.Games.Skills;

namespace LibplanetConsole.ClientServices.Games;

public static class SkillFactory
{
    public static SkillBase Create(Character character, SkillInfo skillInfo)
    {
        return new AttackSkill(character, skillInfo);
    }
}
