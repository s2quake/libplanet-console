using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using OnBoarding.ConsoleHost.Games;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("stage")]
sealed class StageAction : ActionBase
{
    public StageInfo StageInfo { get; set; } = StageInfo.Empty;

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values.Add(nameof(StageInfo), StageInfo.ToBencodex());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        StageInfo = new StageInfo((Dictionary)values[nameof(StageInfo)]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var stageInfo = StageInfo;
        var playerAddress = stageInfo.Player.Address;
        var previousState = context.PreviousState;
        var stageAccount = previousState.GetAccount(playerAddress);
        var seed = context.RandomSeed;
        var stage = new Stage(stageInfo, seed);
        while (stage.IsEnded == false)
        {
            stage.Update();
        }
        var playerInfo = (PlayerInfo)stage.Player;
        stageAccount = stageAccount.SetState(playerAddress, playerInfo.ToBencodex());
        return previousState.SetAccount(playerAddress, stageAccount);
    }
}
