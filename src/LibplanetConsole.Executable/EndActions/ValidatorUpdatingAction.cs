using Libplanet.Action;
using Libplanet.Action.State;

namespace LibplanetConsole.Executable.EndActions;

[ActionType("validator-updating")]
sealed class ValidatorUpdatingAction : ActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.GetService(typeof(IValidatorContext)) is IValidatorContext validatorContext)
        {
            Console.WriteLine("ValidatorUpdatingAction executed!");
        }
        return world;
    }
}
