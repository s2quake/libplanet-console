using Bencodex.Types;
using Libplanet.Action;
using Libplanet.Crypto;
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

    public override string ToString() => $"[{Address}] '{Name}'";

    public PrivateKey PrivateKey => _privateKey;

    public PublicKey PublicKey => _privateKey.PublicKey;

    public Address Address { get; }

    public PlayerInfo? PlayerInfo { get; set; }

    public bool IsOnline { get; private set; }

    public TextWriter Out { get; set; } = Console.Out;

    public void Login(SwarmHost swarmHost)
    {
        if (IsOnline == true)
            throw new InvalidOperationException($"{this} is already online.");

        PlayerInfo = GetPlayerInfo(swarmHost, Address);
        IsOnline = true;
        Out.WriteLine($"{this} is logged in.");
    }

    public void Logout()
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");

        PlayerInfo = null;
        IsOnline = false;
        Out.WriteLine($"{this} is logged out.");
    }

    public PlayerInfo GetPlayerInfo()
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");
        if (PlayerInfo == null)
            throw new InvalidOperationException($"{this} does not have character.");
        return PlayerInfo;
    }

    public GamePlayRecord[] GetGameHistory(SwarmHost swarmHost)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");

        var blockChain = swarmHost.BlockChain;
        return GamePlayRecord.GetGamePlayRecords(blockChain, Address).ToArray();
    }

    public void Refresh(SwarmHost swarmHost)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");

        PlayerInfo = GetPlayerInfo(swarmHost, Address);
    }

    public async Task<long> PlayGameAsync(SwarmHost swarmHost, CancellationToken cancellationToken)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");
        if (PlayerInfo == null)
            throw new InvalidOperationException($"{this} does not have character.");
        if (PlayerInfo.Life <= 0)
            throw new InvalidOperationException($"{this} 's character has died.");

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
        await Out.WriteLineAsync($"{this} requests to play a game.");
        var block = await swarmHost.AddTransactionAsync(this, actions, cancellationToken);
        Refresh(swarmHost);
        await Out.WriteLineAsync($"{this} played the game.");
        return block.Index;
    }

    public Task ReplayGameAsync(SwarmHost swarmHost, int tick, CancellationToken cancellationToken)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");
        if (PlayerInfo == null)
            throw new InvalidOperationException($"{this} does not have character.");

        var blockIndex = PlayerInfo.BlockIndex;
        return ReplayGameAsync(swarmHost, blockIndex, tick, cancellationToken);
    }

    public async Task ReplayGameAsync(SwarmHost swarmHost, long blockIndex, int tick, CancellationToken cancellationToken)
    {
        var address = Address;
        var block = swarmHost.BlockChain[blockIndex];
        if (GamePlayRecord.GetGamePlayRecord(block, Address) is not { } gamePlayRecord)
            throw new ArgumentException($"'Block #{block.Index}' does not have {nameof(StageInfo)}.");

        var stageInfo = gamePlayRecord.GetStageInfo();
        var seed = gamePlayRecord.GetSeed();
        var @out = Out;
        var stage = new Stage(stageInfo, seed, @out);
        await stage.PlayAsync(tick, cancellationToken);
        var playerInfo = (PlayerInfo)stage.Player;
        await Out.WriteLineAsJsonAsync(playerInfo);
    }

    public async Task CreateCharacterAsync(SwarmHost swarmHost, CancellationToken cancellationToken)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");
        if (PlayerInfo != null)
            throw new InvalidOperationException($"{this} already has character.");

        var characterCreationAction = new CharacterCreationAction()
        {
            UserAddress = Address,
            PlayerInfo = PlayerInfo.CreateNew(Name),
        };
        await Out.WriteLineAsync($"{this} requests to create a character.");
        await swarmHost.AddTransactionAsync(this, new IAction[] { characterCreationAction }, cancellationToken);
        Refresh(swarmHost);
        await Out.WriteLineAsync($"{this} created the character.");
    }

    public async Task ReviveCharacterAsync(SwarmHost swarmHost, CancellationToken cancellationToken)
    {
        if (IsOnline == false)
            throw new InvalidOperationException($"{this} is not online.");
        if (PlayerInfo == null)
            throw new InvalidOperationException($"{this} does not have character.");
        if (PlayerInfo.Life > 0)
            throw new InvalidOperationException($"{this} 's is not dead.");

        var characterResurrectionAction = new CharacterResurrectionAction()
        {
            UserAddress = Address,
        };
        await Out.WriteLineAsync($"{this} requests to revive a character.");
        await swarmHost.AddTransactionAsync(this, new IAction[] { characterResurrectionAction }, cancellationToken);
        Refresh(swarmHost);
        await Out.WriteLineAsync($"{this} revived the character.");
    }

    public static PlayerInfo? GetPlayerInfo(SwarmHost swarmHost, Address address)
    {
        var blockChain = swarmHost.BlockChain;
        var worldState = blockChain.GetWorldState();
        var account = worldState.GetAccount(address);
        if (account.GetState(UserStates.PlayerInfo) is Dictionary values)
        {
            return new PlayerInfo(values);
        }
        return null;
    }
}
