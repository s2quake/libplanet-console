using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Client;

internal sealed class ApplicationInfoProvider
    : InfoProviderBase<IHostApplicationLifetime>
{
    private readonly ApplicationInfo _info;

    public ApplicationInfoProvider(IApplicationOptions options)
        : base("Application")
    {
        _info = new()
        {
            Port = options.Port,
            NodeEndPoint = options.NodeEndPoint,
            LogPath = options.LogPath,
        };
    }

    protected override object? GetInfo(IHostApplicationLifetime obj) => _info;
}
