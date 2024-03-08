using LibplanetConsole.Executable.Games.Serializations;
using LibplanetConsole.Executable.Games.Skills;

namespace LibplanetConsole.Executable.Games;

static class SkillFactory
{
    public static SkillBase Create(Character character, SkillInfo skillInfo)
    {
        return new AttackSkill(character, skillInfo);
    }
}
