using Libplanet.Crypto;
using LibplanetConsole.Executable;

namespace LibplanetConsole.Tests;

public class UnitTest1
{
    [Fact]
    public async Task GamePlayTestAsync()
    {
        var privateKey = new PrivateKey();
        var validatorKeys = new PublicKey[] { privateKey.PublicKey };
        var swarmHost = new SwarmHost("swarm", privateKey, validatorKeys, string.Empty);
        var users = new User[1000];
        for (var i = 0; i < users.Length; i++)
        {
            users[i] = new User($"User{i}");
        }
        var cancellationToken = CancellationToken.None;
        await swarmHost.StartAsync(cancellationToken);
        await Task.Delay(1000);
        for (var i = 0; i < users.Length; i++)
        {
            users[i].Login(swarmHost);
        }
        await Task.WhenAll(users.Select(item => item.CreateCharacterAsync(swarmHost, cancellationToken)));
        // user.Login(swarmHost);
        // await user.CreateCharacterAsync(swarmHost, cancellationToken);
        // var playerInfo1 = user.PlayerInfo!;
        // await user.PlayGameAsync(swarmHost, cancellationToken);
        // var playerInfo2 = user.PlayerInfo!;
        // Assert.NotEqual(playerInfo1.Life, playerInfo2.Life);
        // Assert.NotEqual(0, playerInfo2.Experience);
        await swarmHost.DisposeAsync();
    }
}