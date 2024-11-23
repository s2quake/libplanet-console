using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Runs the libplanet-console")]
[CommandExample("run --end-point localhost:5000 --node-count 4 --client-count 2")]
internal sealed class RunCommand
    : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    private Application? _application;
    private PortGenerator? _portGenerator;
    private PortGroup? _ports;

    [CommandProperty]
    [CommandSummary("Specifies the port of the libplanet-console")]
    [NonNegative]
    public int Port { get; init; }

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 4)]
#endif
    [CommandSummary("Specifies the number of nodes to run")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the private keys of the nodes to run")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    public string[] Nodes { get; init; } = [];

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 2)]
#endif
    [CommandSummary("Specifies the number of clients to run. Default is 2")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("Specifies the private keys of the clients to run")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    public string[] Clients { get; init; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Genesis))]
    [CommandSummary("Specifies the file path to load the genesis block")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [CommandSummary("Specifies a hexadecimal genesis string")]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty("module-path")]
    [CommandSummary("Specifies the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    [Category("Genesis")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Specifies the type name of the IActionProvider")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    [Category("Genesis")]
    public string ActionProviderType { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the directory path to store log")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and client processes will not start.")]
    public bool NoProcess { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and client processes start in a new window")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool NewWindow { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the console does not attach to the target process after starting " +
                    "the node and client processes")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool Detach { get; set; }

    [CommandProperty("apv-path")]
    [CommandPropertyExclusion(nameof(AppProtocolVersion))]
    [CommandSummary("Specifies the file path to load the AppProtocolVersion")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string AppProtocolVersionPath { get; init; } = string.Empty;

    [CommandProperty("apv")]
    [CommandSummary("Specifies the AppProtocolVersion")]
    [CommandPropertyExclusion(nameof(AppProtocolVersionPath))]
    public string AppProtocolVersion { get; init; } = string.Empty;

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        var portGenerator = _portGenerator
            ?? throw new InvalidOperationException("PortGenerator is not initialized.");
        var ports = _ports ?? throw new InvalidOperationException("PortGroup is not initialized.");
        var nodeOptions = GetNodeOptions(GetNodes(), portGenerator);
        var clientOptions = GetClientOptions(GetClients(), portGenerator);
        var repository = new Repository(ports, nodeOptions, clientOptions)
        {
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
        };
        options.LogPath = GetFullPath(LogPath);
        options.Nodes = repository.Nodes;
        options.Clients = repository.Clients;
        options.Genesis = Genesis;
        options.GenesisPath = GetFullPath(GenesisPath);
        options.ActionProviderModulePath = ActionProviderModulePath;
        options.ActionProviderType = ActionProviderType;
        options.AppProtocolVersion = AppProtocolVersion;
        options.AppProtocolVersionPath = GetFullPath(AppProtocolVersionPath);
        options.NoProcess = NoProcess;
        options.NewWindow = NewWindow;
        options.Detach = Detach;
        options.BlocksyncPort = ports[4];
        options.ConsensusPort = ports[5];

        if (options.Genesis == string.Empty && options.GenesisPath == string.Empty)
        {
            var genesis = repository.CreateGenesis(
                genesisKey: new PrivateKey(), DateTimeOffset.UtcNow);
            options.Genesis = ByteUtil.Hex(genesis);
        }

        if (options.AppProtocolVersion == string.Empty &&
            options.AppProtocolVersionPath == string.Empty)
        {
            options.AppProtocolVersion
                = Libplanet.Net.AppProtocolVersion.Sign(new(), 1).Token;
        }

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        if (_application is not null)
        {
            throw new InvalidOperationException("The application is already running.");
        }

        try
        {
            var builder = WebApplication.CreateBuilder();
            var services = builder.Services;
            var application = new Application(builder);
            var portGenerator = new PortGenerator(Port);
            var ports = portGenerator.Next();
            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(ports[0], o => o.Protocols = HttpProtocols.Http2);
                options.ListenLocalhost(ports[1], o => o.Protocols = HttpProtocols.Http1AndHttp2);
            });
            services.AddSingleton<IConfigureOptions<ApplicationOptions>>(this);
            _ports = ports;
            _portGenerator = portGenerator;
            _application = application;
            await _application.RunAsync(cancellationToken);
        }
        catch (CommandParsingException e)
        {
            e.Print(System.Console.Out);
            Environment.Exit(1);
        }
        finally
        {
            _application = null;
        }
    }

    private static ClientOptions[] GetClientOptions(
        PrivateKey[] clientPrivateKeys, PortGenerator portGenerator)
    {
        return [.. clientPrivateKeys.Select(CreateClientOptions)];

        ClientOptions CreateClientOptions(PrivateKey privateKey)
        {
            var ports = portGenerator.Next();
            return new ClientOptions
            {
                EndPoint = GetLocalHost(ports[0]),
                PrivateKey = privateKey,
            };
        }
    }

    private NodeOptions[] GetNodeOptions(
        PrivateKey[] nodePrivateKeys, PortGenerator portGenerator)
    {
        return [.. nodePrivateKeys.Select(CreateNodeOptions)];

        NodeOptions CreateNodeOptions(PrivateKey privateKey)
        {
            var ports = portGenerator.Next();
            return new NodeOptions
            {
                EndPoint = GetLocalHost(ports[0]),
                PrivateKey = privateKey,
                ActionProviderModulePath = ActionProviderModulePath,
                ActionProviderType = ActionProviderType,
                BlocksyncPort = ports[4],
                ConsensusPort = ports[5],
            };
        }
    }

    private PrivateKey[] GetNodes()
    {
        if (Nodes.Length > 0)
        {
            return [.. Nodes.Select(item => new PrivateKey(item))];
        }

        return [.. Enumerable.Range(0, NodeCount).Select(item => new PrivateKey())];
    }

    private PrivateKey[] GetClients()
    {
        if (Clients.Length > 0)
        {
            return [.. Clients.Select(item => new PrivateKey(item))];
        }

        return [.. Enumerable.Range(0, ClientCount).Select(item => new PrivateKey())];
    }
}
