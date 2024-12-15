using Libplanet.Action.State;
using LibplanetConsole.Common;
using Nekoyume;
using Nekoyume.Module;

namespace LibplanetConsole.Node.Guild.BlockActions;

internal sealed class TransferGoldToRewardPool : BlockActionBase
{
    protected override IWorld OnExecute(IActionContext context)
    {
        var world = context.PreviousState;
        var goldCurrency = world.GetGoldCurrency();
        world = world.TransferAsset(
            context, Addresses.GoldCurrency, Addresses.RewardPool, goldCurrency * 1);
        return world;
    }
}
