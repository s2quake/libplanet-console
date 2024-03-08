using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using LibplanetConsole.Executable.Games.Serializations;

namespace LibplanetConsole.Executable.Actions;

[ActionType("leader-board")]
sealed class LeaderBoardAction : ActionBase
{
    public const int MaxLength = 5;

    public Address UserAddress { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        return values.Add(nameof(UserAddress), UserAddress.ToByteArray());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        UserAddress = new Address(values[nameof(UserAddress)]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var userAddress = UserAddress;
        var previousState = context.PreviousState;
        var account = previousState.GetAccount(UserAddress);
        var worldAccount = previousState.GetAccount(WorldAccounts.Default);
        if (account.GetState(UserStates.PlayerInfo) is not Dictionary values)
        {
            throw new InvalidOperationException($"User '{userAddress}' does not have character.");
        }
        var playerInfo = new PlayerInfo(values);
        if (playerInfo.Life > 0)
        {
            var rankInfo = new RankInfo()
            {
                Address = userAddress,
                Level = playerInfo.Level,
                Experience = playerInfo.Experience,
            };
            var rankInfos = new RankInfoCollection(worldAccount)
            {
                rankInfo,
            };
            rankInfos.Slice(MaxLength);
            worldAccount = worldAccount.SetState(WorldStates.LeaderBoard, rankInfos.ToBencodex());
            account = account.SetState(UserStates.PlayerInfo, playerInfo.ToBencodex());
            previousState = previousState.SetAccount(WorldAccounts.Default, worldAccount);
            return previousState.SetAccount(userAddress, account);
        }
        return previousState;
    }
}
