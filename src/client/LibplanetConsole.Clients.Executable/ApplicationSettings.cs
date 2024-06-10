using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Executable;

[ApplicationSettings]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the Client Service will run. " +
                    "If omitted, host is 127.0.0.1 and port is set to random.")]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the client. " +
                    "If omitted, a random private key is used.")]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    public int ParentProcessId { get; init; }

    [CommandProperty("seed")]
    [CommandSummary("Use --node-end-point as the Seed EndPoint. " +
                    "Get the EndPoint of the Node to connect to from Seed.")]
    [CommandPropertyCondition(nameof(NodeEndPoint), "", IsNot = true)]
    public bool IsSeed { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    public bool NoREPL { get; init; }

    public static implicit operator ApplicationOptions(ApplicationSettings settings)
    {
        var endPoint = EndPointUtility.ParseWithFallback(settings.EndPoint);
        var privateKey = PrivateKeyUtility.ParseWithFallback(settings.PrivateKey);
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = settings.ParentProcessId,
            IsSeed = settings.IsSeed,
            NodeEndPoint = DnsEndPointUtility.GetSafeEndPoint(settings.NodeEndPoint),
            LogPath = GetFullPath(settings.LogPath),
            NoREPL = settings.NoREPL,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    public static ApplicationSettings Parse(string[] args)
    {
        var options = new ApplicationSettings();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, commandSettings);
        parser.Parse(args);
        return options;
    }
}
