using System.Net;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Executable;

[ApplicationSettings]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("The endpoint of the console to run." +
                    "If omitted, one of the random ports will be used.")]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to run.\n" +
                    "If omitted, the default value is 4.\n" +
                    "If --nodes option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Nodes), null, OnSet = true)]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run.\n" +
                    "Example: --nodes \"key1,key2,...\"")]
    [CommandPropertyCondition(nameof(NodeCount), null, OnSet = true)]
    public string[] Nodes { get; init; } = [];

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to run.\n" +
                    "If omitted, the default value is 2.\n" +
                    "If --clients option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Clients), null, OnSet = true)]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run.\n" +
                    "Example: --clients \"key1,key2,...\"")]
    [CommandPropertyCondition(nameof(ClientCount), null, OnSet = true)]
    public string[] Clients { get; init; } = [];

    [CommandProperty]
    [CommandSummary("The directory path to store data of each node. " +
                    "If omitted, the data is stored in memory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes will not run.")]
    public bool NoProcess { get; set; }

    [CommandPropertySwitch]
    [CommandSummary($"If set, the node and the client processes start in a new window.\n" +
                    $"This option cannot be used with --no-process option.")]
    [CommandPropertyCondition(nameof(NoProcess), false)]
    public bool NewWindow { get; set; }

    [CommandPropertySwitch]
    [CommandSummary("If set, the node and the client processes are detached from the console.\n" +
                    "This option cannot be used with --no-process option.\n" +
                    "And this option is only available if the --new-window option is set.")]
    [CommandPropertyCondition(nameof(NewWindow), true)]
    public bool Detach { get; set; }

    [CommandPropertySwitch('m', useName: true)]
    [CommandSummary("If set, The service does not start automatically " +
                    "when the node and client processes are executed.\n" +
                    $"This option cannot be used with --no-process or --detach option.")]
    [CommandPropertyCondition(nameof(NoProcess), false)]
    [CommandPropertyCondition(nameof(Detach), false)]
    public bool ManualStart { get; set; }

    public static implicit operator ApplicationOptions(ApplicationSettings settings)
    {
        var endPoint = settings.GetEndPoint();
        return new ApplicationOptions(endPoint)
        {
            Nodes = settings.GetNodes(),
            Clients = settings.GetClients(),
            StoreDirectory = GetFullPath(settings.StorePath),
            LogDirectory = GetFullPath(settings.LogPath),
            NoProcess = settings.NoProcess,
            NewWindow = settings.NewWindow,
            Detach = settings.Detach,
            ManualStart = settings.ManualStart,
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

    private EndPoint GetEndPoint()
    {
        if (EndPoint != string.Empty)
        {
            return EndPointUtility.Parse(EndPoint);
        }

        return DnsEndPointUtility.Next();
    }
}
