using System.Net;
using System.Reflection;
using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Consoles;

public record class ApplicationOptions
{
    [CommandProperty]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty(InitValue = 4)]
    [CommandSummary("The number of nodes to run.")]
    public int NodeCount { get; init; }

    [CommandProperty(InitValue = 2)]
    [CommandSummary("The number of clients to run.")]
    public int ClientCount { get; init; }

    [CommandProperty]
    [CommandPropertyCondition(nameof(Volatile), false)]
    [CommandSummary("The directory path to store state of each node." +
                    "If omitted, a directory named '.store' is created in the current directory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandPropertySwitch]
    [CommandSummary("Indicates whether the state is stored in memory.")]
    public bool Volatile { get; init; }

    public static ApplicationOptions Parse(string[] args)
    {
        var options = new ApplicationOptions();
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

    public ApplicationOptions GetActualOptions()
    {
        var storePath = Volatile == true ? string.Empty : GetStoreDirectory(StorePath);

        return this with
        {
            EndPoint = EndPointUtility.ToString(GetEndPoint(this)),
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

    private static string GetStoreDirectory(string storePath)
    {
        if (storePath != string.Empty)
        {
            return storePath;
        }

        var assemblyName = Assembly.GetEntryAssembly()?.GetName().Name ??
            throw new InvalidOperationException("Failed to get the entry assembly name.");

        return Path.Combine(Directory.GetCurrentDirectory(), ".store", assemblyName);
    }
}
