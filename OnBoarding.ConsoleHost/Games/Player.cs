using System.Collections;
using System.ComponentModel.Composition;
using JSSoft.Library.Terminals;
using Libplanet.Action;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Games;

sealed class Player : Character
{
    private readonly Address _address;
    private readonly long _experience;

    public Player(PublicKey publicKey, long life)
        : base(life)
    {
        _address = publicKey.ToAddress();
        Skills =
        [
            new AttackSkill(this, maxCoolTime: 3) { CoolTime = 0L },
        ];
        DisplayName = TerminalStringBuilder.GetString($"{this}", TerminalColorType.Blue);
    }

    public override ISkill[] Skills { get; }

    public override string DisplayName { get; }

    public static long GetExperience(long level)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(level, 0);

        return (long)(Math.Pow(level, 2) * 100);
    }

    public override string ToString()
    {
        return $"P:{_address}"[..8];
    }

    public override bool IsEnemyOf(Character character) => character is Monster;
}
