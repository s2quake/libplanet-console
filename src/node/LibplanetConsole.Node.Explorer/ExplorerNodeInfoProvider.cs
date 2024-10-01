using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Explorer;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExplorerNodeInfoProvider(ExplorerNode explorerNode)
    : InfoProviderBase<IApplication>(nameof(ExplorerNode))
{
    protected override object? GetInfos(IApplication obj) => explorerNode.Info;
}
