using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes.Databases;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class DatabaseNodeInfoProvider(DatabaseNode explorerNode)
    : InfoProviderBase<IApplication>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(IApplication obj)
    {
        yield return (nameof(DatabaseNode), new
        {
            explorerNode.Info.EndPoint,
            Url = $"http://{explorerNode.Info.EndPoint}/ui/playground",
            explorerNode.IsRunning,
        });
    }
}
