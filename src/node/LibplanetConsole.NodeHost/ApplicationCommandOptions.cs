using System.Net;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Nodes;

namespace LibplanetConsole.NodeHost;

public sealed record class ApplicationCommandOptions
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
    [CommandSummary("The directory path to store data." +
                    "If omitted, the data is stored in memory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates array of validators' public keys in the genesis block.\n" +
                    "If omitted, genesis validators is derived from " +
                    "the private key of the node.\n" +
                    "If genesis block does not need to create, this option is ignored.")]
    public string[] GenesisValidators { get; init; } = [];

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    public static implicit operator ApplicationOptions(ApplicationCommandOptions options)
    {
        var endPoint = options.GetEndPoint();
        var privateKey = options.GetPrivateKey();
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = options.ParentProcessId,
            AutoStart = options.AutoStart,
            NodeEndPoint = DnsEndPointUtility.GetSafeEndPoint(options.NodeEndPoint),
            GenesisValidators = options.GetGenesisValidators(privateKey.PublicKey),
            StorePath = GetFullPath(options.StorePath),
            LogPath = GetFullPath(options.LogPath),
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
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

    private PublicKey[] GetGenesisValidators(PublicKey publicKey)
    {
        if (GenesisValidators.Length > 0)
        {
            return [.. GenesisValidators.Select(PublicKeyUtility.Parse)];
        }

        return [publicKey];
    }
}
