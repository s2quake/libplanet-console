using Libplanet.Action;
using Libplanet.Action.State;

namespace LibplanetConsole.Executable.EndActions;

[ActionType("reward")]
sealed class RewardAction : ActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        // Console.WriteLine("RewardAction executed!");
        return world;
    }
}
