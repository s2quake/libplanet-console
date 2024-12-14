using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node.Explorer;

internal sealed class ExplorerInfoProvider(IExplorer explorer)
    : InfoProviderBase<IHostApplicationLifetime>("Explorer")
{
    protected override object? GetInfo(IHostApplicationLifetime obj)
    {
        return new
        {
            Url = $"{explorer.Url}",
        };
    }
}
