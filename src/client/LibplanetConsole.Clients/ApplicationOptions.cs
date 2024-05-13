using System.Net;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Clients;

public record class ApplicationOptions
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
    [CommandPropertyCondition(nameof(NodeEndPoint), "", IsInvert = true)]
    public bool IsSeed { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; init; } = string.Empty;

    public static ApplicationOptions Parse(string[] args)
    {
        var options = new ApplicationOptions();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(options, commandSettings);
        parser.Parse(args);
        return options;
    }

    public ApplicationOptions GetActualOptions()
    {
        var endPoint = GetEndPoint(EndPoint);
        var privateKey = GetPrivateKey(PrivateKey);

        return this with
        {
            EndPoint = endPoint,
            PrivateKey = privateKey,
        };
    }

    public async Task<EndPoint> GetEndPointAsync(CancellationToken cancellationToken)
    {
        if (NodeEndPoint == string.Empty)
        {
            throw new InvalidOperationException($"{nameof(NodeEndPoint)} is not set.");
        }

        if (IsSeed == true)
        {
            return await SeedUtility.GetNodeEndPointAsync(NodeEndPoint, cancellationToken);
        }

        return DnsEndPointUtility.Parse(NodeEndPoint);
    }

    private static string GetEndPoint(string endPoint)
    {
        if (endPoint != string.Empty)
        {
            return endPoint;
        }

        return DnsEndPointUtility.ToString(DnsEndPointUtility.Next());
    }

    private static string GetPrivateKey(string privateKey)
    {
        if (privateKey != string.Empty)
        {
            return privateKey;
        }

        return PrivateKeyUtility.ToString(new());
    }
}
