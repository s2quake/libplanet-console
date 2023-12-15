using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Action.State;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Actions;

[ActionType("character-resurrection")]
sealed class CharacterResurrectionAction : ActionBase
{
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
        if (account.GetState(UserStates.PlayerInfo) is not Dictionary values)
            throw new InvalidOperationException($"User '{userAddress}' does not have character.");
        var playerInfo1 = new PlayerInfo(values);
        if (playerInfo1.Life > 0)
            throw new InvalidOperationException($"User '{userAddress}' 's is not dead.");
        var playerInfo2 = playerInfo1 with { Life = playerInfo1.MaxLife };
        account = account.SetState(UserStates.PlayerInfo, playerInfo2.ToBencodex());
        return previousState.SetAccount(userAddress, account);
    }
}
