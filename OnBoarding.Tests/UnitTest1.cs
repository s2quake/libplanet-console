using Libplanet.Action;
using Libplanet.Crypto;
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
        var users = new UserCollection(count: 10);
        var validators = new PrivateKey[] { new(), new(), new(), new() };
        var swarmHosts = new SwarmHostCollection(validators);
        await swarmHosts.InitializeAsync(cancellationToken: default);

        var user = users[random.Next(users.Count)];
        var swarmHost = swarmHosts[random.Next(swarmHosts.Count)];
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
        var count = swarmHost.BlockChain.Count;
        swarmHost.StageTransaction(user, new IAction[] { stageAction });
        while (swarmHost.BlockChain.Count != count)
        {
            await Task.Delay(1);
        }
        await swarmHosts.DisposeAsync();
    }
}