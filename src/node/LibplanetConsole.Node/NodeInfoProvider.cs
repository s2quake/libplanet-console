using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class NodeInfoProvider(Node node) : InfoProviderBase<ApplicationBase>
{
    protected override IEnumerable<(string Name, object? Value)> GetInfos(ApplicationBase obj)
    {
        yield return (nameof(Node), node.Info);
    }
}
