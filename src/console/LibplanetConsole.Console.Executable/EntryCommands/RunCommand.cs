using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.DataAnnotations;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Options;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
[CommandExample("run --end-point localhost:5000 --node-count 4 --client-count 2")]
internal sealed class RunCommand
    : CommandAsyncBase, IConfigureOptions<ApplicationOptions>
{
    [CommandProperty]
    [CommandSummary("The port of the libplanet-console. " +
                    "If omitted, a random port is used.")]
    [NonNegative]
    public int Port { get; init; }

#if DEBUG
    [CommandProperty(InitValue = 2)]
#else
    [CommandProperty(InitValue = 4)]
#endif
    [CommandSummary("The number of nodes to run. If omitted, 4 nodes are run.\n" +
                    "Mutually exclusive with '--nodes' option.")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run. ex) --nodes \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--node-count' option.")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    public string[] Nodes { get; init; } = [];

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 2)]
#endif
    [CommandSummary("The number of clients to run. If omitted, 2 clients are run.\n" +
                    "Mutually exclusive with '--clients' option.")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run. ex) --clients \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--client-count' option.")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    public string[] Clients { get; init; } = [];

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Genesis))]
    [CommandSummary("Indicates the file path to load the genesis block.\n" +
                    "Mutually exclusive with '--genesis' option.")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [CommandSummary("Indicates a hexadecimal genesis string. If omitted, a random genesis block " +
                    "is used.\nMutually exclusive with '--genesis-path' option.")]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store log.")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes will not run.")]
    public bool NoProcess { get; set; }

    [CommandPropertySwitch]
    [CommandSummary($"If set, the node and the client processes start in a new window.\n" +
                    $"This option cannot be used with --no-process option.")]
    [CommandPropertyExclusion(nameof(NoProcess))]
    public bool NewWindow { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes are detached from the console.\n" +
                    "This option cannot be used with --no-process option.\n" +
                    "And this option is only available if the --new-window option is set.")]
    [CommandPropertyExclusion(nameof(NewWindow))]
    public bool Detach { get; set; }

    void IConfigureOptions<ApplicationOptions>.Configure(ApplicationOptions options)
    {
        var portGenerator = new PortGenerator(Port);
        var port = portGenerator.Current;
        var nodeOptions = GetNodeOptions(GetNodes(), portGenerator);
        var clientOptions = GetClientOptions(GetClients(), portGenerator);
        var repository = new Repository(port, nodeOptions, clientOptions);
        options.LogPath = GetFullPath(LogPath);
        options.Nodes = repository.Nodes;
        options.Clients = repository.Clients;
        options.Genesis = Genesis;
        options.GenesisPath = GetFullPath(GenesisPath);
        options.NoProcess = NoProcess;
        options.NewWindow = NewWindow;
        options.Detach = Detach;

        if (options.Genesis == string.Empty && options.GenesisPath == string.Empty)
        {
            var genesis = repository.CreateGenesis(
                genesisKey: new PrivateKey(), DateTimeOffset.UtcNow);
            options.Genesis = ByteUtil.Hex(genesis);
        }

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
            e.Print(System.Console.Out);
            Environment.Exit(1);
        }
    }

    private static NodeOptions[] GetNodeOptions(
        PrivateKey[] nodePrivateKeys, PortGenerator portGenerator)
    {
        return [.. nodePrivateKeys.Select(key => new NodeOptions
        {
            EndPoint = GetLocalHost(portGenerator.Next()),
            PrivateKey = key,
        })];
    }

    private static ClientOptions[] GetClientOptions(
        PrivateKey[] clientPrivateKeys, PortGenerator portGenerator)
    {
        return [.. clientPrivateKeys.Select(key => new ClientOptions
        {
            EndPoint = GetLocalHost(portGenerator.Next()),
            PrivateKey = key,
        })];
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
