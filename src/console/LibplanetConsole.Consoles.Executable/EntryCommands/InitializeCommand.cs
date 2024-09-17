using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;

namespace LibplanetConsole.Consoles.Executable.EntryCommands;

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
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The endpoint of the libplanet-console. " +
                    "If omitted, a random endpoint is used.")]
    [AppEndPoint]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The private key of the genesis block. " +
                    "if omitted, a random private key is used.")]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty("date-time")]
    [CommandSummary("The timestamp of the genesis block. ex) \"2021-01-01T00:00:00Z\"")]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to create. If omitted, 4 nodes are created.\n" +
                    "Mutually exclusive with '--nodes' option.")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to create. ex) --nodes \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--node-count' option.")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    [AppPrivateKeyArray]
    public string[] Nodes { get; init; } = [];

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to create. If omitted, 2 clients are created.\n" +
                    "Mutually exclusive with '--clients' option.")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to create. ex) --clients \"key1,key2,...\"\n" +
                    "Mutually exclusive with '--client-count' option.")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    [AppPrivateKeyArray]
    public string[] Clients { get; init; } = [];

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command does not output any information.")]
    public bool Quiet { get; set; }

    protected override void OnExecute()
    {
        var genesisKey = AppPrivateKey.ParseOrRandom(GenesisKey);
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var prevEndPoint = EndPoint != string.Empty ? endPoint : null;
        var nodeOptions = GetNodeOptions(ref prevEndPoint);
        var clientOptions = GetClientOptions(ref prevEndPoint);
        var outputPath = Path.GetFullPath(OutputPath);
        var dateTimeOffset = DateTimeOffset.UtcNow;
        var repository = new Repository(endPoint, nodeOptions, clientOptions)
        {
            Genesis = BlockUtility.SerializeBlock(BlockUtility.CreateGenesisBlock(
                genesisKey: genesisKey,
                validatorKeys: nodeOptions.Select(item => item.PrivateKey.PublicKey).ToArray(),
                dateTimeOffset: dateTimeOffset)),
            LogPath = "app.log",
            LibraryLogPath = "library.log",
        };
        var resolver = new RepositoryPathResolver();
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };
        dynamic info = repository.Save(outputPath, resolver);
        info.GenesisArguments = new
        {
            GenesisKey = AppPrivateKey.ToString(genesisKey),
            Validators = nodeOptions.Select(
                item => AppPublicKey.ToString(item.PrivateKey.PublicKey)),
            Timestamp = dateTimeOffset,
        };

        TextWriterExtensions.WriteLineAsJson(writer, info);
    }

    private NodeOptions[] GetNodeOptions(ref AppEndPoint? prevEndPoint)
    {
        var privateKeys = GetNodes();
        var nodeOptionsList = new List<NodeOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var endPoint = prevEndPoint is not null
                ? new AppEndPoint(prevEndPoint.Host, prevEndPoint.Port + 1) : AppEndPoint.Next();
            var nodeOptions = new NodeOptions
            {
                EndPoint = endPoint,
                PrivateKey = privateKey,
                StorePath = "store",
                LogPath = "app.log",
                LibraryLogPath = "library.log",
            };
            nodeOptionsList.Add(nodeOptions);
            if (prevEndPoint is not null)
            {
                prevEndPoint = endPoint;
            }
        }

        return [.. nodeOptionsList];
    }

    private ClientOptions[] GetClientOptions(ref AppEndPoint? prevEndPoint)
    {
        var privateKeys = GetClients();
        var clientOptionsList = new List<ClientOptions>(privateKeys.Length);
        foreach (var privateKey in privateKeys)
        {
            var endPoint = prevEndPoint is not null
                ? new AppEndPoint(prevEndPoint.Host, prevEndPoint.Port + 1) : AppEndPoint.Next();
            var clientOptions = new ClientOptions
            {
                EndPoint = endPoint,
                PrivateKey = privateKey,
                LogPath = "app.log",
            };
            clientOptionsList.Add(clientOptions);
            if (prevEndPoint is not null)
            {
                prevEndPoint = endPoint;
            }
        }

        return [.. clientOptionsList];
    }

    private AppPrivateKey[] GetNodes()
    {
        if (Nodes.Length > 0)
        {
            return [.. Nodes.Select(item => AppPrivateKey.Parse(item))];
        }

        return [.. Enumerable.Range(0, NodeCount).Select(item => new AppPrivateKey())];
    }

    private AppPrivateKey[] GetClients()
    {
        if (Clients.Length > 0)
        {
            return [.. Clients.Select(item => AppPrivateKey.Parse(item))];
        }

        return [.. Enumerable.Range(0, ClientCount).Select(item => new AppPrivateKey())];
    }
}
