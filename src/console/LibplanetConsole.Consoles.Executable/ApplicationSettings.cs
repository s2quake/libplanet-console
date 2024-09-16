using System.Text.Json.Serialization;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable;

[ApplicationSettings(IsRequired = true)]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("The endpoint of the console to run." +
                    "If omitted, one of the random ports will be used.")]
    [AppEndPoint]
    public string EndPoint { get; init; } = string.Empty;

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 4)]
#endif
    [CommandSummary("The number of nodes to run.\n" +
                    "If omitted, the default value is 4.\n" +
                    "If --nodes option is set, this option is ignored.")]
    [CommandPropertyExclusion(nameof(Nodes))]
    [JsonIgnore]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run.\n" +
                    "Example: --nodes \"key1,key2,...\"")]
    [CommandPropertyExclusion(nameof(NodeCount))]
    [JsonIgnore]
    public string[] Nodes { get; init; } = [];

#if DEBUG
    [CommandProperty(InitValue = 1)]
#else
    [CommandProperty(InitValue = 2)]
#endif
    [CommandSummary("The number of clients to run.\n" +
                    "If omitted, the default value is 2.\n" +
                    "If --clients option is set, this option is ignored.")]
    [CommandPropertyExclusion(nameof(Clients))]
    [JsonIgnore]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run.\n" +
                    "Example: --clients \"key1,key2,...\"")]
    [CommandPropertyExclusion(nameof(ClientCount))]
    [JsonIgnore]
    public string[] Clients { get; init; } = [];

    [CommandProperty]
    [CommandSummary("The directory path to store data of each node. " +
                    "If omitted, the data is stored in memory.")]
    [JsonIgnore]
    public string RepositoryPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store log of the library.")]
    public string LibraryLogPath { get; set; } = string.Empty;

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

    public ApplicationOptions ToOptions(object[] components)
    {
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var nodeOptions = GetNodeOptions(endPoint, GetNodes());
        var clientOptions = GetClientOptions(nodeOptions, GetClients());
        var repository = new Repository(endPoint, nodeOptions, clientOptions);
        return new ApplicationOptions(endPoint)
        {
            LogPath = GetFullPath(LogPath),
            LibraryLogPath = GetFullPath(LibraryLogPath),
            Nodes = repository.Nodes,
            Clients = repository.Clients,
            Genesis = repository.Genesis,
            NoProcess = NoProcess,
            NewWindow = NewWindow,
            Detach = Detach,
            Components = components,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    public static ApplicationSettings Parse(string[] args)
    {
        var options = new ApplicationSettings();
        var parserSettings = new CommandSettings()
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, parserSettings);
        parser.Parse(args);
        if (options.NodeCount < 1)
        {
            throw new InvalidOperationException("Node count must be greater than or equal to 1.");
        }

        return options;
    }

    private static NodeOptions[] GetNodeOptions(
        AppEndPoint endPoint, AppPrivateKey[] nodePrivateKeys)
    {
        return [.. nodePrivateKeys.Select(key => new NodeOptions
        {
            EndPoint = AppEndPoint.Next(),
            PrivateKey = key,
            SeedEndPoint = endPoint,
        })];
    }

    private static ClientOptions[] GetClientOptions(
        NodeOptions[] nodeOptions, AppPrivateKey[] clientPrivateKeys)
    {
        return [.. clientPrivateKeys.Select(key => new ClientOptions
        {
            EndPoint = AppEndPoint.Next(),
            NodeEndPoint = Random(nodeOptions).EndPoint,
            PrivateKey = key,
        })];

        static NodeOptions Random(NodeOptions[] nodeOptions)
            => nodeOptions[System.Random.Shared.Next(nodeOptions.Length)];
    }

    private AppPrivateKey[] GetNodes()
    {
        if (Nodes.Length > 0)
        {
            return [.. Nodes.Select(AppPrivateKey.Parse)];
        }

        return [.. Enumerable.Range(0, NodeCount).Select(item => new AppPrivateKey())];
    }

    private AppPrivateKey[] GetClients()
    {
        if (Clients.Length > 0)
        {
            return [.. Clients.Select(AppPrivateKey.Parse)];
        }

        return [.. Enumerable.Range(0, ClientCount).Select(item => new AppPrivateKey())];
    }
}
