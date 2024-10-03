using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Exit the application.")]
internal sealed class ExitCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => application.Cancel();
}
