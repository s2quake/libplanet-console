using JSSoft.Library.Terminals;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

sealed class Monster : Character
{
    private readonly Address _address = new PrivateKey().ToAddress();

    public Monster(long life)
        : base(life)
    {
        Skills =
        [
            new AttackSkill(this, maxCoolTime: 10) { CoolTime = RandomUtility.GetNext(10) },
        ];
        DisplayName = TerminalStringBuilder.GetString($"{this}", TerminalColorType.Red);
    }

    public override Address Address => _address;

    public override ISkill[] Skills { get; }

    public override string DisplayName { get; }

    public override string ToString()
    {
        return $"M:{_address}"[..8];
    }

    public override bool IsEnemyOf(Character character) => character is Player;
}
