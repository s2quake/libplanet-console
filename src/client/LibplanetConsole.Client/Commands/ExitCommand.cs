using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Client.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Exit the application.")]
[method: ImportingConstructor]
internal sealed class ExitCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => application.Cancel();
}
