using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using OnBoarding.ConsoleHost.Extensions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class PlayerCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly UserCollection _users;

    [ImportingConstructor]
    public PlayerCommand(Application application)
    {
        _application = application;
        _users = application.GetService<UserCollection>()!;
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        var currentIndex = _application.CurrentIndex;
        for (var i = 0; i < _users.Count; i++)
        {
            var item = _users[i];
            var isCurrent = currentIndex == i ? "O" : " ";
            sb.AppendLine($"{isCurrent} [{i}]-{item.Address}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(SwarmProperties), nameof(SwarmProperties.Index))]
    public void Info(int blockIndex = -1)
    {
        var playerInfo = _application.GetPlayerInfo(SwarmProperties.Index, blockIndex);
        Out.WriteLineAsJson(playerInfo);
    }
}
