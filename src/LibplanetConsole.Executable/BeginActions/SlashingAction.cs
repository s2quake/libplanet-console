using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;

namespace LibplanetConsole.Executable.BeginActions;

[ActionType("slashing")]
sealed class SlashingAction : ActionBase
{
    public Address Validator { get; set; }

    public long InfractionHeight { get; set; }

    public long Power { get; set; }

    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        if (context.GetService(typeof(IEvidenceContext)) is IEvidenceContext evidenceContext)
        {
            // Console.WriteLine("SlashingAction executed!");
            // var powerIndex = ValidatorPowerIndexCtrl.FetchValidatorPowerIndex(world);
            // int qwer = 0;

        }
        else
        {
            throw new NotSupportedException();
        }
        return world;
    }
}
