using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Explorer;

[Export(typeof(IInfoProvider))]
internal sealed class ExplorerInfoProvider(Explorer explorer)
    : InfoProviderBase<IApplication>(nameof(Explorer))
{
    protected override object? GetInfo(IApplication obj) => explorer.Info;
}
