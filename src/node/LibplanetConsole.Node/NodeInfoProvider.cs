using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class NodeInfoProvider(Node node) : InfoProviderBase<ApplicationBase>(nameof(Node))
{
    protected override object? GetInfo(ApplicationBase obj) => node.Info;
}
