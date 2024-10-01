using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Explorer;

[Export(typeof(IInfoProvider))]
internal sealed class ExplorerNodeInfoProvider
    : InfoProviderBase<ExplorerNodeContent>
{
    public ExplorerNodeInfoProvider()
        : base("Explorer")
    {
    }

    protected override object? GetInfos(ExplorerNodeContent obj)
    {
        return new
        {
            obj.EndPoint,
            obj.IsRunning,
            Url = obj.IsRunning ? $"http://{obj.EndPoint}/ui/playground" : string.Empty,
        };
    }
}
