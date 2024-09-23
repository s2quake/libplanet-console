using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Print node application information.")]
[method: ImportingConstructor]
internal sealed class InfoCommand(IApplication application) : CommandBase
{
    protected override void OnExecute()
    {
        var info = InfoUtility.GetInfo(serviceProvider: application, obj: application);
        Out.WriteLineAsJson(info);
    }
}
