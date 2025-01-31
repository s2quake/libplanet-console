using LibplanetConsole.Common;
using Microsoft.Extensions.Hosting;

namespace LibplanetConsole.Console;

internal sealed class ApplicationInfoProvider : InfoProviderBase<IHostApplicationLifetime>
{
    public ApplicationInfoProvider(IApplicationOptions options)
        : base("Application")
    {
        Info = new()
        {
            LogPath = options.LogPath,
            GenesisHash = options.GenesisBlock.Hash.ToString(),
            AppProtocolVersion = options.AppProtocolVersion,
            NoProcess = options.NoProcess,
            Detach = options.Detach,
            NewWindow = options.NewWindow,
        };
    }

    public ApplicationInfo Info { get; }

    protected override object? GetInfo(IHostApplicationLifetime obj) => Info;
}
