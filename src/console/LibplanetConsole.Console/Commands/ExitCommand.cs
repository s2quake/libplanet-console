using JSSoft.Commands;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Exit the application.")]
internal sealed class ExitCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => application.Cancel();
}
