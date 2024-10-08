using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Console;

internal sealed class ApplicationInfoProvider : InfoProviderBase<IHostApplicationLifetime>
{
    private readonly ApplicationInfo _info;

    public ApplicationInfoProvider(ApplicationOptions options)
        : base("Application")
    {
         _info = new()
        {
            EndPoint = options.EndPoint,
            LogPath = options.LogPath,
            NoProcess = options.NoProcess,
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };
    }

    protected override object? GetInfo(IHostApplicationLifetime obj) => _info;
}
