using Bencodex.Types;
using Libplanet.Action;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("dummy")]
sealed class DummyAction : ActionBase
{
    protected override void OnInitialize(Dictionary values)
    {
        base.OnInitialize(values);
    }
}
