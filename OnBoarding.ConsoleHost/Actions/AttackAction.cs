using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("attack")]
sealed class AttackAction : ActionBase
{
    public Address Source { get; set; }

    public Address Target { get; set; }

    public long Damage { get; set; }

    public long TargetLife { get; set; }
}
