using Libplanet.Action;
using OnBoarding.ConsoleHost;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.Tests;

public class UnitTest1
{
    [Fact]
    public async Task TestAsync()
    {
        var random = new Random();
        var users = new UserCollection();
        var swarmHosts = new SwarmHostCollection(users);
        await swarmHosts.InitializeAsync(default);

        var index = random.Next(users.Count);
        var user = users[index];
        var swarmHost = swarmHosts[index];

        var playerInfo = user.GetPlayerInfo(swarmHost);
        var stageInfo = new StageInfo
        {
            Address = new(),
            Player = playerInfo,
            Monsters = MonsterInfo.Create(10),
        };
        var stageAction = new StageAction
        {
            StageInfo = stageInfo,
        };
        swarmHost.StageTransaction(user, new IAction[] { stageAction });
        await swarmHosts.DisposeAsync();
    }
}