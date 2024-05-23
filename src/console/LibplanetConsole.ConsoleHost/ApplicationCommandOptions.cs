using System.Net;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Consoles;

namespace LibplanetConsole.ConsoleHost;

public sealed record class ApplicationCommandOptions
{
    [CommandProperty]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to run.\n" +
                    "If omitted, the default value is 4.\n" +
                    "If --nodes option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Nodes), null, IsSet = true)]
    public int NodeCount { get; init; }

    [CommandProperty]
    [CommandSummary("The private keys of the nodes to run.")]
    [CommandPropertyCondition(nameof(NodeCount), null, IsSet = true)]
    public string[] Nodes { get; init; } = [];

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to run.\n" +
                    "If omitted, the default value is 2.\n" +
                    "If --clients option is set, this option is ignored.")]
    [CommandPropertyCondition(nameof(Clients), null, IsSet = true)]
    public int ClientCount { get; init; }

    [CommandProperty(InitValue = new string[] { })]
    [CommandSummary("The private keys of the clients to run.")]
    [CommandPropertyCondition(nameof(ClientCount), null, IsSet = true)]
    public string[] Clients { get; init; } = [];

    [CommandProperty]
    [CommandPropertyCondition(nameof(Volatile), false)]
    [CommandSummary("The directory path to store data of each node. " +
                    "If omitted, the data is stored in memory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    public static implicit operator ApplicationOptions(ApplicationCommandOptions options)
    {
        var endPoint = options.GetEndPoint();
        return new ApplicationOptions(endPoint)
        {
            Nodes = options.GetNodes(),
            Clients = options.GetClients(),
            StoreDirectory = GetFullPath(options.StorePath),
            LogDirectory = GetFullPath(options.LogPath),
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    public static ApplicationCommandOptions Parse(string[] args)
    {
        var options = new ApplicationCommandOptions();
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
