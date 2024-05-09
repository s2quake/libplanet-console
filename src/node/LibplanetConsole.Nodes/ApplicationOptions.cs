using System.Net;
using System.Reflection;
using JSSoft.Commands;
using JSSoft.Communication;
using LibplanetConsole.Common;

namespace LibplanetConsole.Nodes;

public record class ApplicationOptions
{
    [CommandProperty]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    public int ParentProcessId { get; init; }

    [CommandPropertySwitch('a')]
    public bool AutoStart { get; init; } = false;

    [CommandProperty]
    public string SeedEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyCondition(nameof(Volatile), false)]
    public string StorePath { get; init; } = string.Empty;

    [CommandPropertySwitch]
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
