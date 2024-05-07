using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.ClientHost.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Print client information.")]
[method: ImportingConstructor]
internal sealed class InfoCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => Out.WriteLineAsJson(application.Info);
}
