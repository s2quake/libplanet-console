using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Node;

namespace LibplanetConsole.Console.Executable.EntryCommands;

[CommandSummary("Create a new repository to run Libplanet nodes and clients from the console.")]
internal sealed class InitializeCommand : CommandBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("The directory path used to initialize a repository.")]
    [Path(
        Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The endpoint of the libplanet-console. " +
                    "If omitted, a random endpoint is used.")]
    [EndPoint]
    public string EndPoint { get; set; } = string.Empty;

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

    protected override void OnExecute()
    {
        var genesisKey = PrivateKeyUtility.ParseOrRandom(GenesisKey);
        var endPoint = EndPointUtility.ParseOrNext(EndPoint);
        var prevEndPoint = EndPoint != string.Empty ? endPoint : null;
        var nodeOptions = GetNodeOptions(ref prevEndPoint);
        var clientOptions = GetClientOptions(ref prevEndPoint);
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
        var genesis = Repository.CreateGenesis(genesisOptions);
        var repository = new Repository(endPoint, nodeOptions, clientOptions)
        {
            Genesis = genesis,
            LogPath = "log",
        };
        var resolver = new RepositoryPathResolver();
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };
        dynamic info = repository.Save(outputPath, resolver);
        info.GenesisArguments = new
        {
            GenesisKey = PrivateKeyUtility.ToString(genesisKey),
            Validators = nodeOptions.Select(
                item => item.PrivateKey.PublicKey.ToHex(compress: false)),
            Timestamp = dateTimeOffset,
            ActionProviderModulePath,
            ActionProviderType,
        };

        TextWriterExtensions.WriteLineAsJson(writer, info);
    }

    private NodeOptions[] GetNodeOptions(ref EndPoint? prevEndPoint)
    {
        var privateKeys = GetNodes();
        var nodeOptionsList = new List<NodeOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var endPoint = prevEndPoint ?? EndPointUtility.NextEndPoint();
            EndPointUtility.NextEndPoint();
            var nodeOptions = new NodeOptions
            {
                EndPoint = endPoint,
                PrivateKey = privateKey,
                StorePath = "store",
                LogPath = "log",
                ActionProviderModulePath = ActionProviderModulePath,
                ActionProviderType = ActionProviderType,
            };
            nodeOptionsList.Add(nodeOptions);
            if (prevEndPoint is not null)
            {
                prevEndPoint = endPoint;
            }
        }

        return [.. nodeOptionsList];
    }

    private ClientOptions[] GetClientOptions(ref EndPoint? prevEndPoint)
    {
        var privateKeys = GetClients();
        var clientOptionsList = new List<ClientOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var endPoint = prevEndPoint ?? EndPointUtility.NextEndPoint();
            var clientOptions = new ClientOptions
            {
                EndPoint = endPoint,
                PrivateKey = privateKey,
                LogPath = "log",
            };
            clientOptionsList.Add(clientOptions);
            if (prevEndPoint is not null)
            {
                prevEndPoint = endPoint;
            }
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
