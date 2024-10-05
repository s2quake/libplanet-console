using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class NodeInfoProvider(Node node)
    : InfoProviderBase<IHostApplicationLifetime>(nameof(Node))
{
    protected override object? GetInfo(IHostApplicationLifetime obj) => node.Info;
}
