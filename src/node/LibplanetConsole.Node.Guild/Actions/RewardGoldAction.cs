using Lib9c;
using Libplanet.Action.State;
using LibplanetConsole.Common;
using Nekoyume;

namespace LibplanetConsole.Node.Guild.Actions;

public sealed class RewardGoldAction : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var states = context.PreviousState;
        var gasCurrency = Currencies.Mead;
        var usedGas = states.GetBalance(Addresses.GasPool, gasCurrency);
        var defaultReward = gasCurrency * 5;
        var halfOfUsedGas = usedGas.DivRem(2).Quotient;
        var gasToBurn = usedGas - halfOfUsedGas;
        var miningReward = halfOfUsedGas + defaultReward;
        states = states.MintAsset(context, Addresses.GasPool, defaultReward);
        if (gasToBurn.Sign > 0)
        {
            states = states.BurnAsset(context, Addresses.GasPool, gasToBurn);
        }

        states = states.TransferAsset(
            context, Addresses.GasPool, Addresses.RewardPool, miningReward);

        return states;
    }
}
