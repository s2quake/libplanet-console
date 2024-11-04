using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Create a new repository to run Libplanet nodes and clients from the console.")]
internal sealed class InitializeCommand : CommandAsyncBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("The directory path used to initialize a repository.")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The port of the libplanet-console. " +
                    "If omitted, a random port is used.")]
    [NonNegative]
    public int Port { get; set; }

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 4)]
#endif
    [CommandSummary("The number of nodes to create. If omitted, 4 nodes are created.\n" +
                    "Mutually exclusive with '--nodes' option.")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to create. ex) --nodes \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--node-count' option.")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    [PrivateKeyArray]
    public string[] Nodes { get; init; } = [];

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 2)]
#endif
    [CommandSummary("The number of clients to create. If omitted, 2 clients are created.\n" +
                    "Mutually exclusive with '--clients' option.")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to create. ex) --clients \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--client-count' option.")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    [PrivateKeyArray]
    public string[] Clients { get; init; } = [];

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command does not output any information.")]
    public bool Quiet { get; set; }

    [CommandProperty]
    [CommandSummary("The private key of the genesis block. " +
                    "if omitted, a random private key is used.")]
    [PrivateKey]
    [Category("Genesis")]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty("timestamp")]
    [CommandSummary("The timestamp of the genesis block. ex) \"2021-01-01T00:00:00Z\"")]
    [Category("Genesis")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandProperty("module-path")]
    [CommandSummary("Indicates the path or the name of the assembly that provides " +
                    "the IActionProvider.")]
    [Category("Genesis")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Indicates the type name of the IActionProvider.")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    [Category("Genesis")]
    public string ActionProviderType { get; set; } = string.Empty;

    [CommandProperty("port-spacing", InitValue = PortGenerator.DefaultSpace)]
    [CommandSummary("Specifies the spacing between ports. Default is 10. " +
                    "This option is only used when --port-generation-mode is set to " +
                    "'sequential'. If --port-generation-mode is set to 'random', " +
                    "the value of this option is 10'")]
    [Category("Network")]
    [Range(10, 10000)]
    public int PortSpacing { get; set; }

    [CommandProperty("port-generation-mode")]
    [CommandSummary("Specifies the mode for generating ports: Random or Sequential.")]
    [Category("Network")]
    public PortGenerationMode PortGenerationMode { get; set; } = PortGenerationMode.Sequential;

    [CommandProperty("apv-private-key")]
    [CommandSummary("The private key of the signer of the AppProtocolVersion. If omitted, " +
                    "a random private key is used.")]
    [PrivateKey]
    [Category("AppProtocolVersion")]
    public string APVPrivateKey { get; set; } = string.Empty;

    [CommandProperty("apv-version", InitValue = 1)]
    [CommandSummary("The version number of the AppProtocolVersion. Default is 1.")]
    [Category("AppProtocolVersion")]
    public int APVVersion { get; set; }

    [CommandProperty("apv-extra")]
    [CommandSummary("The extra data to be included in the AppProtocolVersion.")]
    [Category("AppProtocolVersion")]
    public string APVExtra { get; set; } = string.Empty;

    protected override async Task OnExecuteAsync(CancellationToken cancellationToken)
    {
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };
        var info = await ExecuteAsync(cancellationToken);
        await TextWriterExtensions.WriteLineAsJsonAsync(writer, info, cancellationToken);
    }

    private async Task<dynamic> ExecuteAsync(CancellationToken cancellationToken)
    {
        using var progress = new CommandProgress();
        var portGenerator = new PortGenerator(Port);
        var genesisKey = PrivateKeyUtility.ParseOrRandom(GenesisKey);
        var port = portGenerator.Current;
        var nodeOptions = GetNodeOptions(portGenerator, port);
        var clientOptions = GetClientOptions(portGenerator);
        var outputPath = Path.GetFullPath(RepositoryPath);
        var dateTimeOffset = DateTimeOffset != DateTimeOffset.MinValue
            ? DateTimeOffset : DateTimeOffset.UtcNow;
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = genesisKey,
            Validators = nodeOptions.Select(item => item.PrivateKey.PublicKey).ToArray(),
            Timestamp = dateTimeOffset,
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
        };
        var apvPrivateKey = PrivateKeyUtility.ParseOrRandom(APVPrivateKey);
        var repository = new Repository(port, nodeOptions, clientOptions)
        {
            Genesis = Repository.CreateGenesis(genesisOptions),
            AppProtocolVersion = Repository.CreateAppProtocolVersion(
                apvPrivateKey, APVVersion, APVExtra),
            LogPath = "log",
        };
        var resolver = new RepositoryPathResolver();
        dynamic info = await repository.SaveAsync(
            outputPath, resolver, cancellationToken, progress);
        info.GenesisArguments = new
        {
            GenesisKey = PrivateKeyUtility.ToString(genesisKey),
            Validators = nodeOptions.Select(
                item => item.PrivateKey.PublicKey.ToHex(compress: false)),
            Timestamp = dateTimeOffset,
            ActionProviderModulePath,
            ActionProviderType,
        };
        info.AppProtocolVersionArguments = new
        {
            PrivateKey = PrivateKeyUtility.ToString(apvPrivateKey),
            Version = APVVersion,
            Extra = APVExtra,
        };

        return info;
    }

    private NodeOptions[] GetNodeOptions(PortGenerator portGenerator, int port)
    {
        var privateKeys = GetNodes();
        var nodeOptionsList = new List<NodeOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var nodeOptions = new NodeOptions
            {
                EndPoint = GetLocalHost(portGenerator.Next()),
                PrivateKey = privateKey,
                StorePath = "store",
                LogPath = "log",
                SeedEndPoint = GetLocalHost(port),
                ActionProviderModulePath = ActionProviderModulePath,
                ActionProviderType = ActionProviderType,
            };
            nodeOptionsList.Add(nodeOptions);
        }

        return [.. nodeOptionsList];
    }

    private ClientOptions[] GetClientOptions(PortGenerator portGenerator)
    {
        var privateKeys = GetClients();
        var clientOptionsList = new List<ClientOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var clientOptions = new ClientOptions
            {
                EndPoint = GetLocalHost(portGenerator.Next()),
                PrivateKey = privateKey,
                LogPath = "log",
            };
            clientOptionsList.Add(clientOptions);
        }

        return [.. clientOptionsList];
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
