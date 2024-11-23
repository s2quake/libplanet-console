using System.ComponentModel;
using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Runs the libplanet-console")]
internal sealed class RunCommand
    : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandProperty]
    [CommandSummary("Specifies the port on which the node will run")]
    [NonNegative]
    public int Port { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the private key of the node")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console")]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the EndPoint of the seed node to connect to")]
    [CommandPropertyExclusion(nameof(IsSingleNode))]
    [EndPoint]
    public string SeedEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the directory path to store data")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Genesis))]
    [CommandSummary("Specifies the file path to load the genesis block")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [CommandSummary("Specifies a hexadecimal genesis string")]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty("apv-path")]
    [CommandPropertyExclusion(nameof(AppProtocolVersion))]
    [CommandSummary("Specifies the file path to load the AppProtocolVersion")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string AppProtocolVersionPath { get; init; } = string.Empty;

    [CommandProperty("apv")]
    [CommandPropertyExclusion(nameof(AppProtocolVersionPath))]
    [CommandSummary("Specifies the AppProtocolVersion")]
    [AppProtocolVersion]
    public string AppProtocolVersion { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the directory path to save logs")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string LogPath { get; init; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the application starts without REPL")]
    public bool NoREPL { get; init; }

    [CommandPropertySwitch("single-node")]
    [CommandPropertyExclusion(nameof(SeedEndPoint))]
    [CommandSummary("If set, the node runs as a single node")]
    public bool IsSingleNode { get; set; }

    [CommandProperty("module-path")]
    [CommandSummary("Specifies the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Specifies the type name of the IActionProvider")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    public string ActionProviderType { get; set; } = string.Empty;

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        options.PrivateKey = PrivateKey;
        options.GenesisPath = GetFullPath(GenesisPath);
        options.Genesis = Genesis;
        options.AppProtocolVersionPath = GetFullPath(AppProtocolVersionPath);
        options.AppProtocolVersion = AppProtocolVersion;
        options.ParentProcessId = ParentProcessId;
        options.SeedEndPoint = SeedEndPoint;
        options.StorePath = GetFullPath(StorePath);
        options.LogPath = GetFullPath(LogPath);
        options.NoREPL = NoREPL;
        options.ActionProviderModulePath = ActionProviderModulePath;
        options.ActionProviderType = ActionProviderType;

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            var application = new Application(builder);
            var port = Port is 0 ? PortUtility.NextPort() : Port;
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });
            services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }
}
