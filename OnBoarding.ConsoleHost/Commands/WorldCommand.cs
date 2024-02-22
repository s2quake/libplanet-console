using System.ComponentModel.Composition;
using Bencodex.Types;
using JSSoft.Commands;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class WorldCommand(Application application) : CommandMethodBase
{
    private readonly Application _application = application;

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    public void LeaderBoard()
    {
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var blockChain = swarmHost.BlockChain;

        var world = blockChain.GetWorldState();
        var worldAccount = world.GetAccountState(WorldAccounts.Default);
        if (worldAccount.GetState(WorldStates.LeaderBoard) is List rankList)
        {
            var rankInfos = RankInfo.GetRankInfos(rankList);
            Out.WriteLineAsJson(rankInfos);
        }
        else
        {
            Out.WriteLine("There is no leader board.");
        }
    }
}
