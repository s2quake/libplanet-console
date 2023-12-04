using System.ComponentModel.Composition;
using Bencodex.Types;
using JSSoft.Library.Commands;
using Libplanet.Action;
using Libplanet.Blockchain;
using Libplanet.Crypto;
using Newtonsoft.Json;
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
        var playerInfo = Player.GetPlayerInfo(_blockChain, Player.CurrentAddress);
        var user = _users.First(item => item.Address == playerInfo.Address);
        var player = new Player(playerInfo);
        var monsters = MonsterCollection.Create(difficulty: 1, count: 10);
        var stage = new Stage(player, monsters);
        var actionList = new List<IAction>(100)
        {
            new StageAction { StageInfo = (StageInfo)stage },
        };
        var stagePlayer = new StagePlayer(stage, Out);
        var actions = await stagePlayer.StartAsync(Tick, cancellationToken);
        // while (cancellationToken.IsCancellationRequested == false && stage.IsEnded == false)
        // {
        //     await Out.WriteLineAsync($"Turn #{turn}");
        //     stage.Update();
        //     actionList.Add(new StageAction { StageInfo = (StageInfo)stage });
        //     turn++;
        //     await Task.Delay(Tick, cancellationToken: default);
        // }
        // if (cancellationToken.IsCancellationRequested == true)
        // {
        //     Error.WriteLine("Play has been canceled.");
        // }
        // else
        // {
        // }
        BlockChainUtils.AppendNew(_blockChain, user, _users, actions);
    }

    public bool CanPlay => Player.GetPlayerInfo(_blockChain, Player.CurrentAddress).Life > 0;
}
