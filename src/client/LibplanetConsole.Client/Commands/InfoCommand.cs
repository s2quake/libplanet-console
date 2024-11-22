using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client.Commands;

[CommandSummary("Prints the information of the client application")]
internal sealed class InfoCommand(
    IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime)
    : CommandBase
{
    protected override void OnExecute()
    {
        var info = InfoUtility.GetInfo(serviceProvider: serviceProvider, obj: applicationLifetime);
        Out.WriteLineAsJson(info);
    }
}
