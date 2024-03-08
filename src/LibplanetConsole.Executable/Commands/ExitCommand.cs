using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Exit the application.")]
[method: ImportingConstructor]
sealed class ExitCommand(Application application) : CommandBase
{
    protected override void OnExecute()
    {
        application.Cancel();
    }
}
