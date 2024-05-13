using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Print node information.")]
[method: ImportingConstructor]
internal sealed class InfoCommand(IApplication application) : CommandBase
{
    protected override void OnExecute() => Out.WriteLineAsJson(application.Info);
}
