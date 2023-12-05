using JSSoft.Library.Terminals;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

sealed class Monster : Character
{
    private readonly Address _address = new PrivateKey().ToAddress();

    public Monster(MonsterInfo monsterInfo)
        : base(monsterInfo)
    {
        Skills = monsterInfo.Skills.Select(item => SkillFactory.Create(this, item)).ToArray();
        DisplayName = TerminalStringBuilder.GetString($"{this}", TerminalColorType.Red);
    }

    public override ISkill[] Skills { get; }

    public override string DisplayName { get; }

    public override string ToString()
    {
        return $"M:{_address}"[..8];
    }

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
