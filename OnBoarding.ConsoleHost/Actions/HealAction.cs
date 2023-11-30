using Bencodex.Types;
using Libplanet.Action;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("heal")]
sealed class HealAction : ActionBase
{
    protected override void OnInitialize(Dictionary values)
    {
        base.OnInitialize(values);
    }
}
