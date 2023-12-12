using Libplanet.Action;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.Tests;

public class UnitTest1
{
    [Fact]
    public async Task GamePlayTestAsync()
    {
        var random = new Random();
        var users = new UserCollection(count: 10);
        var validators = new PrivateKey[] { new() };
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
            UserAddress = user.Address,
        };
        var count = swarmHost.BlockChain.Count;
        swarmHost.StageTransaction(user, new IAction[] { stageAction });
        while (swarmHost.BlockChain.Count == count)
        {
            await Task.Delay(1);
        }
        var playerInfo2 = user.GetPlayerInfo(swarmHost);
        Assert.NotEqual(playerInfo.Life, playerInfo2.Life);
        Assert.NotEqual(0, playerInfo2.Experience);
        await swarmHosts.DisposeAsync();
    }
}