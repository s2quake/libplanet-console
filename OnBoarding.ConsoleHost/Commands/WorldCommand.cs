using System.ComponentModel.Composition;
using Bencodex.Types;
using JSSoft.Commands;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class WorldCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly SwarmHostCollection _swarmHosts;

    [ImportingConstructor]
    public WorldCommand(Application application, SwarmHostCollection swarmHosts)
    {
        _application = application;
        _swarmHosts = swarmHosts;
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    public void LeaderBoard()
    {
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var blockChain = swarmHost.BlockChain;

        var world = blockChain.GetWorldState();
        var worldAccount = world.GetAccount(WorldAccounts.Default);
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
