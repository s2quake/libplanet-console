using System.ComponentModel.Composition;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[method: ImportingConstructor]
[CommandSummary("Exit the application.")]
sealed class ExitCommand(Application application) : CommandBase
{
    private readonly Application _application = application;

    protected override void OnExecute()
    {
        _application.Cancel();
    }
}
