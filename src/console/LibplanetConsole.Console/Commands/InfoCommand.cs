using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Console.Commands;

[CommandSummary("Print console application information.")]
internal sealed class InfoCommand(IApplication application) : CommandBase
{
    protected override void OnExecute()
    {
        var info = InfoUtility.GetInfo(serviceProvider: application, obj: application);
        Out.WriteLineAsJson(info);
    }
}
