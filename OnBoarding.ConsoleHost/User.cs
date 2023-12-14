using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
using Nito.AsyncEx.Interop;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Extensions;
using OnBoarding.ConsoleHost.Games;
using OnBoarding.ConsoleHost.Games.Serializations;

namespace OnBoarding.ConsoleHost;

sealed class User
{
    private readonly PrivateKey _privateKey;

    public User(string name)
    {
        _privateKey = PrivateKeyUtility.Create(name);
        Address = _privateKey.ToAddress();
        Name = name;
    }

    public string Name { get; }

    public override string ToString() => $"{_privateKey.PublicKey}";

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address { get; }

    public PlayerInfo? PlayerInfo { get; set; }

    public bool IsOnline { get; private set; }

    public void Login(SwarmHost swarmHost)
    {
        if (IsOnline == true)
            throw new InvalidOperationException($"'{Name}' is already online.");

        PlayerInfo = GetPlayerInfo(swarmHost, Address);
        IsOnline = true;
        Console.WriteLine($"User '{Address}' is logged in.");
    }

    public void Logout()
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");

        PlayerInfo = null;
        IsOnline = false;
        Console.WriteLine($"User '{Address}' is logged out.");
    }

    public void Refresh(SwarmHost swarmHost)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");

        PlayerInfo = GetPlayerInfo(swarmHost, Address);
    }

    public Task ReplayGameAsync(SwarmHost swarmHost, int tick, TextWriter @out, CancellationToken cancellationToken)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"'{Name}' is not online.");

        return ReplayGameAsync(swarmHost, PlayerInfo!.BlockIndex, tick, @out, cancellationToken);
    }

    public async Task PlayGameAsync(SwarmHost swarmHost, CancellationToken cancellationToken)
    {
        var playerInfo = PlayerInfo!;
        var monsterCount = RandomUtility.GetNext(8, 12);
        var stageInfo = new StageInfo
        {
            Player = playerInfo,
            Monsters = MonsterInfo.Create(monsterCount),
        };
        var gamePlayAction = new GamePlayAction
        {
            StageInfo = stageInfo,
            UserAddress = Address,
        };
        var leaderBoardAction = new LeaderBoardAction
        {
            UserAddress = Address,
        };
        var actions = new IAction[]
        {
            gamePlayAction,
            leaderBoardAction,
        };
        await swarmHost.AddTransactionAsync(this, actions, cancellationToken);
        Refresh(swarmHost);
        Console.WriteLine($"User '{Address}' played the game.");
    }

    public async Task ReplayGameAsync(SwarmHost swarmHost, long blockIndex, int tick, TextWriter @out, CancellationToken cancellationToken)
    {
        var address = Address;
        var block = swarmHost.BlockChain[blockIndex];
        if (GamePlayRecord.GetGamePlayRecord(block, Address) is not { } gamePlayRecord)
            throw new ArgumentException($"'Block #{block.Index}' does not have {nameof(StageInfo)}.");

        var stageInfo = gamePlayRecord.GetStageInfo();
        var seed = gamePlayRecord.GetSeed();
        var stage = new Stage(stageInfo, seed, @out);
        await stage.PlayAsync(tick, cancellationToken);
        var playerInfo = (PlayerInfo)stage.Player;
        @out.WriteLineAsJson(playerInfo);
    }

    public async Task CreateCharacter(SwarmHost swarmHost, CancellationToken cancellationToken)
    {
        var characterCreationAction = new CharacterCreationAction()
        {
            UserAddress = Address,
            PlayerInfo = PlayerInfo.CreateNew(Name),
        };
        await swarmHost.AddTransactionAsync(this, new IAction[] { characterCreationAction }, cancellationToken);
        Refresh(swarmHost);
        Console.WriteLine($"User '{Address}' created the character.");
    }

    public static PlayerInfo? GetPlayerInfo(SwarmHost swarmHost, Address address)
    {
        var blockChain = swarmHost.BlockChain;
        var worldState = blockChain.GetWorldState();
        var account = worldState.GetAccount(address);
        if (account.GetState(PlayerStates.PlayerInfo) is Dictionary values)
        {
            return new PlayerInfo(values);
        }
        return null;
    }
}
