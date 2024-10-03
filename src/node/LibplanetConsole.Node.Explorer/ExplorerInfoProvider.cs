using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Explorer;

internal sealed class ExplorerInfoProvider(Explorer explorer)
    : InfoProviderBase<IApplication>(nameof(Explorer))
{
    protected override object? GetInfo(IApplication obj) => explorer.Info;
}
