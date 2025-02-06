using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Node;

internal sealed class ApplicationInfoProvider : InfoProviderBase<IHostApplicationLifetime>
{
    private readonly ApplicationInfo _info;

    public ApplicationInfoProvider(IApplicationOptions options)
        : base("Application")
    {
        _info = new()
        {
            HubUrl = options.HubUrl,
            StorePath = options.StorePath,
            LogPath = options.LogPath,
            ParentProcessId = options.ParentProcessId,
            IsSingleNode = options.IsSingleNode,
        };
    }

    protected override object? GetInfo(IHostApplicationLifetime obj) => _info;
}
