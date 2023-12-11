using System.Collections.Concurrent;
using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;
using OnBoarding.ConsoleHost.Extensions;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
sealed class UserCommand : CommandMethodBase
{
    private readonly Application _application;
    private readonly UserCollection _users;

    [ImportingConstructor]
    public UserCommand(Application application)
    {
        _application = application;
        _users = application.GetService<UserCollection>()!;
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _users.Count; i++)
        {
            var item = _users[i];
            sb.AppendLine($"[{i}]-{item.Address}");
        }
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.SwarmIndex))]
    [CommandMethodStaticProperty(typeof(IndexProperties), nameof(IndexProperties.UserIndex))]
    public void Info(int blockIndex = -1)
    {
        var user = _application.GetUser(IndexProperties.SwarmIndex);
        var swarmHost = _application.GetSwarmHost(IndexProperties.SwarmIndex);
        var playerInfo = user.GetPlayerInfo(swarmHost, blockIndex);
        Out.WriteLineAsJson(playerInfo);
    }
}
