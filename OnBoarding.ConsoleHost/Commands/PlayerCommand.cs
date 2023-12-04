using System.ComponentModel.Composition;
using System.Text;
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
sealed class PlayerCommand : CommandMethodBase
{
    private readonly BlockChain _blockChain;
    private readonly UserCollection _users;

    [ImportingConstructor]
    public PlayerCommand(Application application)
    {
        _blockChain = application.GetService<BlockChain>()!;
        _users = application.GetService<UserCollection>()!;
        Player.CurrentAddress = _users[0].Address;
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _users.Count; i++)
        {
            var item = _users[i];
            var isCurrent = Player.CurrentAddress == item.Address ? "O" : " ";
            sb.AppendLine($"{isCurrent} [{i}]-{item.Address}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public void Info()
    {
        var playerInfo = Player.GetPlayerInfo(_blockChain, Player.CurrentAddress);
        var s = JsonConvert.SerializeObject(playerInfo, Formatting.Indented);
        Out.WriteLine(s);
    }

    public bool CanRevive => Player.GetPlayerInfo(_blockChain, Player.CurrentAddress).Life <= 0;

    [CommandMethod]
    public void Revive()
    {
        
    }
}
