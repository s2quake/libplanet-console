using JSSoft.Terminals;
using LibplanetConsole.ClientServices.Games.Serializations;

namespace LibplanetConsole.ClientServices.Games;

public sealed class Monster : Character
{
    public Monster(MonsterInfo monsterInfo)
        : base(monsterInfo)
    {
        Skills = monsterInfo.Skills.Select(item => SkillFactory.Create(this, item)).ToArray();
        DisplayName = TerminalStringBuilder.GetString($"{Name}", TerminalColorType.BrightRed);
    }

    public override ISkill[] Skills { get; }

    public override string DisplayName { get; }

    public override bool IsEnemyOf(Character character) => character is Player;

    protected override void OnDeal(Character attacker, long amount)
    {
        base.OnDeal(attacker, amount);
        if (attacker is Player player && IsDead == true)
        {
            player.Experience += 10;
        }
    }
}
