using System.ComponentModel.Composition;
using System.Text;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
sealed class UserCommand(Application application, UserCollection users) : CommandMethodBase
{
    private readonly Application _application = application;
    private readonly UserCollection _users = users;

    [CommandMethod]
    public void New(int count = 1)
    {
        var sb = new StringBuilder();
        var index = _users.Count;
        for (var i = 0; i < count; i++)
        {
            var user = _users.AddNew();
            sb.AppendLine($"[{index + i}]: {user}");
        }
        sb.AppendLine($"{count} users were created.");
        Out.Write(sb.ToString());
    }

    [CommandMethod]
    public void List()
    {
        var sb = new StringBuilder();
        for (var i = 0; i < _users.Count; i++)
        {
            var user = _users[i];
            sb.AppendLine($"[{i}]: {user}");
        }
        Out.Write(sb.ToString());
    }
}
