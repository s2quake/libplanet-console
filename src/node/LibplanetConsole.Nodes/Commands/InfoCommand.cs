using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.Extensions;

namespace LibplanetConsole.Nodes.Commands;

[Export(typeof(ICommand))]
[CommandSummary("Print node information.")]
[method: ImportingConstructor]
internal sealed class InfoCommand(IApplication application) : CommandBase
{
    protected override void OnExecute()
    {
        var infoProviders = application.GetService<IEnumerable<IInfoProvider>>();
        var infos = infoProviders.Where(IsAssignableFrom)
                                 .SelectMany(item => item.GetInfos())
                                 .ToDictionary(item => item.Name, item => item.Value);
        Out.WriteLineAsJson(infos);
    }

    private static bool IsAssignableFrom(IInfoProvider infoProvider)
        => typeof(IApplication).IsAssignableFrom(infoProvider.DeclaringType);
}
