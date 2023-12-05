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
    public void Play()
    {
        var playerInfo = Player.GetPlayerInfo(_blockChain, Player.CurrentAddress);
        var user = _users.First(item => item.Address == playerInfo.Address);
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
        BlockChainUtils.AppendNew(_blockChain, user, _users, [stageAction]);
    }

    public bool CanPlay => Player.GetPlayerInfo(_blockChain, Player.CurrentAddress).Life > 0;
}
