namespace OnBoarding.ConsoleHost.Games;

static class SkillFactory
{
    public static SkillBase Create(Character character, SkillInfo skillInfo)
    {
        return new AttackSkill(character, skillInfo);
    }
}
