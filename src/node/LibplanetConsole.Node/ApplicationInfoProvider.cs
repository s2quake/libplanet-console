using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class ApplicationInfoProvider : InfoProviderBase<IHostApplicationLifetime>
{
    private readonly ApplicationInfo _info;

    public ApplicationInfoProvider(ApplicationOptions options)
        : base("Application")
    {
        _info = new()
        {
            Port = options.Port,
            SeedEndPoint = options.SeedEndPoint,
            StorePath = options.StorePath,
            LogPath = options.LogPath,
            ParentProcessId = options.ParentProcessId,
        };
    }

    protected override object? GetInfo(IHostApplicationLifetime obj) => _info;
}
