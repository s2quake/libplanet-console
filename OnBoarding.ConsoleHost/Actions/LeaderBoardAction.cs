// using Bencodex.Types;
// using Libplanet.Action;
// using Libplanet.Action.State;
// using Libplanet.Crypto;
// using OnBoarding.ConsoleHost.Games.Serializations;

// namespace OnBoarding.ConsoleHost.Actions;

// [ActionType("leader-board")]
// sealed class LeaderBoardAction : ActionBase
// {
//     public Address UserAddress { get; set; }

//     protected override Dictionary OnInitialize(Dictionary values)
//     {
//         return values.Add(nameof(UserAddress), UserAddress.ToByteArray());
//     }

//     protected override void OnLoadPlainValue(Dictionary values)
//     {
//         UserAddress = new Address(values[nameof(UserAddress)]);
//     }

//     protected override IWorld OnExecute(IActionContext context)
//     {
//         var userAddress = UserAddress;
//         var previousState = context.PreviousState;
//         var account = previousState.GetAccount(UserAddress);
//         if (account.GetState(PlayerStates.PlayerInfo) is not Dictionary values)
//         {
//             throw new InvalidOperationException($"The character of user '{userAddress}' has already been created.");
//         }
//         var playerInfo = new PlayerInfo(values);
//         playerInfo.BlockIndex = context.BlockIndex;
//         account = account.SetState(PlayerStates.PlayerInfo, playerInfo.ToBencodex());
//         return previousState.SetAccount(userAddress, account);
//     }
// }
