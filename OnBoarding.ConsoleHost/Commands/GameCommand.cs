using System.ComponentModel.Composition;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using OnBoarding.ConsoleHost.Actions;
using OnBoarding.ConsoleHost.Games;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class GameCommand(Application application) : CommandMethodBase
{
    private readonly BlockChain _blockChain = application.GetService<BlockChain>()!;
    private readonly UserCollection _users = application.GetService<UserCollection>()!;

    [CommandProperty(InitValue = 1)]
    public int Tick { get; set; }

    [CommandMethod]
    [CommandMethodValidation(nameof(CanPlay))]
    [CommandMethodProperty(nameof(Tick))]
    public async Task PlayAsync(CancellationToken cancellationToken)
    {
        var user = _users[0];
        var life = GetPlayerLife(user);
        var player = new Player(user.PublicKey, life);
        var monsters = MonsterCollection.Create(difficulty: 1, count: 10);
        var stage = new Stage(player, monsters);
        var turn = 0;
        var actionList = new List<IAction>(100);
        while (cancellationToken.IsCancellationRequested == false && stage.IsEnded == false)
        {
            await Out.WriteLineAsync($"Turn #{turn}");
            stage.Update();
            actionList.Add(new StageAction() { StageInfo = (StageInfo)stage });
            turn++;
            await Task.Delay(Tick, cancellationToken: default);
        }
        if (cancellationToken.IsCancellationRequested == true)
        {
            Error.WriteLine("Play has been canceled.");
        }
        else
        {
            BlockChainUtils.AppendNew(_blockChain, user, _users, [.. actionList]);
        }
    }

    public bool CanPlay => GetPlayerLife(_users[0]) > 0;

    private long GetPlayerLife(User user)
    {
        var block = _blockChain[_blockChain.Count - 1];
        var worldState = _blockChain.GetWorldState(block.Hash);
        var account = worldState.GetAccount(user.Address);
        if (account.GetState(user.Address) is Integer integer)
        {
            return integer;
        }
        return 1000;
    }
}
