using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("character-creation")]
sealed class CharacterCreationAction : ActionBase
{
    public Address UserAddress { get; set; }

    public PlayerInfo? PlayerInfo { get; set; }

    protected override Dictionary OnInitialize(Dictionary values)
    {
        if (PlayerInfo == null)
            throw new InvalidOperationException($"Property '{nameof(PlayerInfo)}' can not be null.");
        return values.Add(nameof(UserAddress), UserAddress.ToByteArray())
                     .Add(nameof(PlayerInfo), PlayerInfo.ToBencodex());
    }

    protected override void OnLoadPlainValue(Dictionary values)
    {
        UserAddress = new Address(values[nameof(UserAddress)]);
        PlayerInfo = new PlayerInfo((Dictionary)values[nameof(PlayerInfo)]);
    }

    protected override IWorld OnExecute(IActionContext context)
    {
        var playerInfo = PlayerInfo!;
        var userAddress = UserAddress;
        var previousState = context.PreviousState;
        var account = previousState.GetAccount(UserAddress);
        if (account.GetState(PlayerStates.PlayerInfo) is not null)
        {
            throw new InvalidOperationException($"The character of user '{userAddress}' has already been created.");
        }
        playerInfo.BlockIndex = context.BlockIndex;
        account = account.SetState(PlayerStates.PlayerInfo, playerInfo.ToBencodex());
        return previousState.SetAccount(userAddress, account);
    }
}
