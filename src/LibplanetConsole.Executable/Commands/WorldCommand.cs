// using System.ComponentModel.Composition;
// using Bencodex.Types;
// using JSSoft.Commands;
// using LibplanetConsole.Games.Serializations;
// using LibplanetConsole.Common;
// using LibplanetConsole.Common.Extensions;

// namespace LibplanetConsole.Executable.Commands;

// [Export(typeof(ICommand))]
// [method: ImportingConstructor]
// internal sealed class WorldCommand(Application application) : CommandMethodBase
// {
//     private readonly Application _application = application;
//
//     [CommandMethod]
//     [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.Node))]
//     public void LeaderBoard()
//     {
//         var node = _application.GetNode(IndexProperties.NodeIndex);
//         var blockChain = node.BlockChain;
//
//         var world = blockChain.GetWorldState();
//         var worldAccount = world.GetAccountState(WorldAccounts.Default);
//         if (worldAccount.GetState(WorldStates.LeaderBoard) is List rankList)
//         {
//             var rankInfos = RankInfo.GetRankInfos(rankList);
//             Out.WriteLineAsJson(rankInfos);
//         }
//         else
//         {
//             Out.WriteLine("There is no leader board.");
//         }
//     }
// }
