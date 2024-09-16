using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;

namespace LibplanetConsole.Consoles.Executable.EntryCommands;

[CommandSummary("Run the Libplanet console.")]
internal sealed class InitializeCommand : CommandBase
{
    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("The directory path to initialize.")]
    public string OutputPath { get; set; } = string.Empty;

    [CommandProperty]
    [AppEndPoint]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [AppPrivateKey]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to run.\n" +
                    "If omitted, the default value is 4.\n" +
                    "If --nodes option is set, this option is ignored.")]
    [CommandPropertyExclusion(nameof(Nodes))]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run.\n" +
                    "Example: --nodes \"key1,key2,...\"")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    [AppPrivateKeyArray]
    public string[] Nodes { get; init; } = [];

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to run.\n" +
                    "If omitted, the default value is 2.\n" +
                    "If --clients option is set, this option is ignored.")]
    [CommandPropertyExclusion(nameof(Clients))]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run.\n" +
                    "Example: --clients \"key1,key2,...\"")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    [AppPrivateKeyArray]
    public string[] Clients { get; init; } = [];

    [CommandPropertySwitch("quiet", 'q')]
    public bool Quiet { get; set; }

    protected override void OnExecute()
    {
        var genesisKey = AppPrivateKey.ParseOrRandom(GenesisKey);
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var prevEndPoint = EndPoint != string.Empty ? endPoint : null;
        var nodeOptions = GetNodeOptions(ref prevEndPoint);
        var clientOptions = GetClientOptions(ref prevEndPoint);
        var outputPath = Path.GetFullPath(OutputPath);
        var repository = new Repository(endPoint, nodeOptions, clientOptions)
        {
            Genesis = BlockUtility.SerializeBlock(BlockUtility.CreateGenesisBlock(
                genesisKey: genesisKey,
                validatorKeys: nodeOptions.Select(item => item.PrivateKey.PublicKey).ToArray(),
                dateTimeOffset: DateTimeOffset.UtcNow)),
            LogPath = "app.log",
            LibraryLogPath = "library.log",
        };
        var resolver = new RepositoryPathResolver();
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };
        dynamic info = repository.Save(outputPath, resolver);

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
