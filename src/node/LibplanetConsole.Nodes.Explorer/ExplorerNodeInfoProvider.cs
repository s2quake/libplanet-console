using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Explorer;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExplorerNodeInfoProvider(ExplorerNode explorerNode)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(ExplorerNode), new
        {
            explorerNode.Info.EndPoint,
            Url = $"http://{explorerNode.Info.EndPoint}/ui/playground",
            explorerNode.IsRunning,
        });
    }
}
