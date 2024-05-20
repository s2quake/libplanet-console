using System.Net;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Clients;
using LibplanetConsole.Common;

namespace LibplanetConsole.ClientHost;

public sealed record class ApplicationCommandOptions
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

    public static implicit operator ApplicationOptions(ApplicationCommandOptions options)
    {
        var endPoint = options.GetEndPoint();
        var privateKey = options.GetPrivateKey();
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = options.ParentProcessId,
            IsSeed = options.IsSeed,
            NodeEndPoint = DnsEndPointUtility.GetSafeEndPoint(options.NodeEndPoint),
        };
    }

    public static ApplicationCommandOptions Parse(string[] args)
    {
        var options = new ApplicationCommandOptions();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, commandSettings);
        parser.Parse(args);
        return options;
    }

    private PrivateKey GetPrivateKey()
    {
        if (PrivateKey != string.Empty)
        {
            return PrivateKeyUtility.Parse(PrivateKey);
        }

        return new();
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
