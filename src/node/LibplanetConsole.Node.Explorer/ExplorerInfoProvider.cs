using System.ComponentModel.Composition;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Explorer;

[Export(typeof(IInfoProvider))]
[method: ImportingConstructor]
internal sealed class ExplorerInfoProvider(Explorer explorer)
    : InfoProviderBase<IApplication>(nameof(Explorer))
{
    protected override object? GetInfo(IApplication obj) => explorer.Info;
}
