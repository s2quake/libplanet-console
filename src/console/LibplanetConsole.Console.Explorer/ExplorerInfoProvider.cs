using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Explorer;

[Export(typeof(IInfoProvider))]
internal sealed class ExplorerInfoProvider
    : InfoProviderBase<ExplorerNodeContent>
{
    public ExplorerInfoProvider()
        : base("Explorer")
    {
    }

    protected override object? GetInfo(ExplorerNodeContent obj)
    {
        return new
        {
            obj.EndPoint,
            obj.IsRunning,
            Url = obj.IsRunning ? $"http://{obj.EndPoint}/ui/playground" : string.Empty,
        };
    }
}
