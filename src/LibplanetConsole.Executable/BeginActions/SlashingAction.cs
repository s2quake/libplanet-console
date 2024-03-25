using Libplanet.Action;
using Libplanet.Action.State;

namespace LibplanetConsole.Executable.BeginActions;

[ActionType("slashing")]
sealed class SlashingAction : ActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.GetService(typeof(IEvidenceContext)) is IEvidenceContext evidenceContext)
        {
            Console.WriteLine("SlashingAction executed!");
        }
        else
        {
            throw new NotSupportedException();
        }
        return world;
    }
}
