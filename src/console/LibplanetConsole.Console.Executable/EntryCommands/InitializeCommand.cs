using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Node;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Creates a new repository to run libplanet nodes and clients via the console")]
internal sealed class InitializeCommand : CommandAsyncBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("Specifies the directory path used to initialize a repository")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the port of the libplanet-console")]
    [NonNegative]
    public int Port { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the private key of the genesis")]
    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 4)]
#endif
    [CommandSummary("Specifies the number of nodes to create. Default is 4")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("Specifies the private keys of the nodes to create")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    [PrivateKeyArray]
    public string[] Nodes { get; init; } = [];

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 2)]
#endif
    [CommandSummary("Specifies the number of clients to create. Default is 2")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("Specifies the private keys of the clients to create")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    [PrivateKeyArray]
    public string[] Clients { get; init; } = [];

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command will not output any information")]
    public bool Quiet { get; set; }

    [CommandProperty("timestamp")]
    [CommandSummary("Specifies the timestamp of the genesis block")]
    [Category("Genesis")]
    public DateTimeOffset DateTimeOffset { get; set; }

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

    [CommandProperty("apv-private-key")]
    [CommandSummary("Specifies the private key of the signer of the AppProtocolVersion")]
    [PrivateKey]
    [Category("AppProtocolVersion")]
    public string APVPrivateKey { get; set; } = string.Empty;

    [CommandProperty("apv-version", InitValue = 1)]
    [CommandSummary("Specifies the version number of the AppProtocolVersion. Default is 1")]
    [Category("AppProtocolVersion")]
    public int APVVersion { get; set; }

    [CommandProperty("apv-extra")]
    [CommandSummary("Specifies the extra data to be included in the AppProtocolVersion")]
    [Category("AppProtocolVersion")]
    public string APVExtra { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the port for the blocksync of the node")]
    [Category("Seed")]
    public int BlocksyncPort { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the port for the consensus of the node")]
    [Category("Seed")]
    public int ConsensusPort { get; set; }

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
        var genesisKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var ports = portGenerator.Next();
        var nodeOptions = GetNodeOptions(portGenerator, ports);
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
        var repository = new Repository(ports, nodeOptions, clientOptions)
        {
            PrivateKey = genesisKey,
            Port = Port,
            Genesis = Repository.CreateGenesis(genesisOptions),
            AppProtocolVersion = Repository.CreateAppProtocolVersion(
                apvPrivateKey, APVVersion, APVExtra),
            LogPath = "log",
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
            BlocksyncPort = BlocksyncPort,
            ConsensusPort = ConsensusPort,
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

    private NodeOptions[] GetNodeOptions(PortGenerator portGenerator, PortGroup consolePorts)
    {
        var privateKeys = GetNodes();
        var nodeOptionsList = new List<NodeOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var ports = portGenerator.Next();
            var nodeOptions = new NodeOptions
            {
                Url = UriUtility.GetLocalHost(ports[0]),
                PrivateKey = privateKey,
                StorePath = "store",
                LogPath = "log",
                HubUrl = UriUtility.GetLocalHost(consolePorts[0]),
                ActionProviderModulePath = ActionProviderModulePath,
                ActionProviderType = ActionProviderType,
                BlocksyncPort = ports[4],
                ConsensusPort = ports[5],
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
            var ports = portGenerator.Next();
            var clientOptions = new ClientOptions
            {
                Url = UriUtility.GetLocalHost(ports[0]),
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
