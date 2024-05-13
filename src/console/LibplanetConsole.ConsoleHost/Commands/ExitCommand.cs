using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Exit the application.")]
[method: ImportingConstructor]
internal sealed class ExitCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => application.Cancel();
}
