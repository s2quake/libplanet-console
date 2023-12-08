using System.ComponentModel.Composition;
using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Exit the application.")]
sealed class ExitCommand : CommandBase
{
    private readonly Application _application;

    [ImportingConstructor]
    public ExitCommand(Application application)
    {
        _application = application;
    }

    protected override void OnExecute()
    {
        _application.Cancel();
    }
}
