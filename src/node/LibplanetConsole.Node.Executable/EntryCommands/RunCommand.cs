using System.ComponentModel;
using System.Diagnostics;
using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Runs the libplanet-console")]
internal sealed class RunCommand : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
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
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the EndPoint of the seed node to connect to")]
    [CommandPropertyExclusion(nameof(IsSingleNode))]
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [EndPoint]
    public string SeedEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the directory path to store data")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Genesis))]
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [CommandSummary("Specifies the file path to load the genesis block")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [CommandSummary("Specifies a hexadecimal genesis string")]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty("apv-path")]
    [CommandPropertyExclusion(nameof(AppProtocolVersion))]
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [CommandSummary("Specifies the file path to load the AppProtocolVersion")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string AppProtocolVersionPath { get; init; } = string.Empty;

    [CommandProperty("apv")]
    [CommandPropertyExclusion(nameof(AppProtocolVersionPath))]
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
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
    [CommandPropertyExclusion(nameof(ConsoleEndPoint))]
    [CommandSummary("If set, the node runs as a single node")]
    public bool IsSingleNode { get; init; }

    [CommandProperty("module-path")]
    [CommandSummary("Specifies the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    public string ActionProviderModulePath { get; init; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Specifies the type name of the IActionProvider")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    public string ActionProviderType { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the end-point of the console to connect to.")]
    [EndPoint]
    public string ConsoleEndPoint { get; init; } = string.Empty;

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        if (ConsoleEndPoint != string.Empty && GenesisPath != string.Empty)
        {
            throw new UnreachableException(
                "Both ConsoleEndPoint and GenesisPath cannot be specified at the same time.");
        }

        if (ConsoleEndPoint != string.Empty && Genesis != string.Empty)
        {
            throw new UnreachableException(
                "Both ConsoleEndPoint and Genesis cannot be specified at the same time.");
        }

        options.PrivateKey = PrivateKey;
        options.GenesisPath = GetFullPath(GenesisPath);
        options.Genesis = Genesis;
        options.AppProtocolVersionPath = GetFullPath(AppProtocolVersionPath);
        options.AppProtocolVersion = AppProtocolVersion;
        options.ParentProcessId = ParentProcessId;
        options.SeedEndPoint = SeedEndPoint;
        options.IsSingleNode = IsSingleNode;

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
            var consoleEndPoint = EndPointUtility.ParseOrDefault(ConsoleEndPoint);
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(port, o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(port + 1, o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });
            services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            if (consoleEndPoint is not null)
            {
                services.AddHostedService<ConsoleHostedService>(s => new(s, port, consoleEndPoint));
                services.AddSingleton<IConfigureOptions<ApplicationOptions>>(
                    _ => new ConsoleConfigureOptions(consoleEndPoint));
            }

            await application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(Console.Out);
            Environment.Exit(1);
        }
    }
}
