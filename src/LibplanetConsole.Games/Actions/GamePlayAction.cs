using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Games;
using LibplanetConsole.Games.Serializations;

namespace LibplanetConsole.Games.Actions;

[ActionType("game-play")]
internal sealed class GamePlayAction : ActionBase
{
    public StageInfo StageInfo { get; set; } = StageInfo.Empty;

    public Address UserAddress { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values.Add(nameof(StageInfo), StageInfo.ToBencodex())
                     .Add(nameof(UserAddress), UserAddress.ToByteArray());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        StageInfo = new StageInfo((Dictionary)values[nameof(StageInfo)]);
        UserAddress = new Address(values[nameof(UserAddress)]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var stageInfo = StageInfo;
        var userAddress = UserAddress;
        var previousState = context.PreviousState;
        var account = previousState.GetAccount(userAddress);
        var seed = context.RandomSeed;
        var playerInfo = PlayStage(stageInfo, seed);
        playerInfo.BlockIndex = context.BlockIndex;
        account = account.SetState(UserStates.PlayerInfo, playerInfo.ToBencodex());
        return previousState.SetAccount(userAddress, account);
    }

    private static PlayerInfo PlayStage(StageInfo stageInfo, int seed)
    {
        var stage = new Stage(stageInfo, seed);
        stage.Play();
        return (PlayerInfo)stage.Player;
    }
}
