using System.Net;
using System.Reflection;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public record class ApplicationOptions
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the Node Service will run. " +
                    "If omitted, host is 127.0.0.1 and port is set to random.")]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    public int ParentProcessId { get; init; }

    [CommandPropertySwitch('a')]
    [CommandSummary("Indicates whether the node starts automatically when the application starts.")]
    public bool AutoStart { get; init; } = false;

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyCondition(nameof(Volatile), false)]
    [CommandSummary("The directory path to store state of each node." +
                    "If omitted, a directory named '.store' is created in the current directory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("Indicates whether the state of the node is stored in memory.")]
    public bool Volatile { get; init; }

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
        var privateKey = GetPrivateKey(PrivateKey);
        var storePath = Volatile == true ? string.Empty : GetStorePath(StorePath, privateKey);

        return this with
        {
            EndPoint = EndPointUtility.ToString(GetEndPoint(this)),
            PrivateKey = privateKey,
            StorePath = storePath,
        };
    }

    private static EndPoint GetEndPoint(ApplicationOptions options)
    {
        if (options.EndPoint != string.Empty)
        {
            return EndPointUtility.Parse(options.EndPoint);
        }

        return DnsEndPointUtility.Next();
    }

    private static string GetPrivateKey(string privateKey)
    {
        if (privateKey != string.Empty)
        {
            return privateKey;
        }

        return PrivateKeyUtility.ToString(new());
    }

    private static string GetStorePath(string storePath, string privateKey)
    {
        if (storePath != string.Empty)
        {
            return storePath;
        }

        var shortAddress = (ShortAddress)PrivateKeyUtility.Parse(privateKey).Address;
        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ??
            throw new InvalidOperationException("Failed to get the entry assembly name.");

        return Path.Combine(Directory.GetCurrentDirectory(), ".store", assemblyName, shortAddress);
    }
}
