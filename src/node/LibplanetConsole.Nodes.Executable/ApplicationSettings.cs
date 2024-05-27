using System.Net;
using JSSoft.Commands;
using Libplanet.Crypto;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[ApplicationSettings]
public sealed class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the Node Service will run. " +
                    "If omitted, host is 127.0.0.1 and port is set to random.")]
    public string EndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    public string PrivateKey { get; set; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    public int ParentProcessId { get; set; }

    [CommandPropertySwitch('a')]
    [CommandSummary("Indicates whether the node starts automatically when the application starts.")]
    public bool AutoStart { get; set; } = false;

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the node to connect to.")]
    public string NodeEndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store data." +
                    "If omitted, the data is stored in memory.")]
    public string StorePath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates array of validators' public keys in the genesis block.\n" +
                    "If omitted, genesis validators is derived from " +
                    "the private key of the node.\n" +
                    "If genesis block does not need to create, this option is ignored.")]
    public string[] GenesisValidators { get; set; } = [];

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    public string LogPath { get; set; } = string.Empty;

    public static implicit operator ApplicationOptions(ApplicationSettings settings)
    {
        var endPoint = settings.GetEndPoint();
        var privateKey = settings.GetPrivateKey();
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = settings.ParentProcessId,
            AutoStart = settings.AutoStart,
            NodeEndPoint = DnsEndPointUtility.GetSafeEndPoint(settings.NodeEndPoint),
            GenesisValidators = settings.GetGenesisValidators(privateKey.PublicKey),
            StorePath = GetFullPath(settings.StorePath),
            LogPath = GetFullPath(settings.LogPath),
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    public static ApplicationSettings Parse(string[] args)
    {
        var settings = new ApplicationSettings();
        var commandSettings = new CommandSettings
        {
            AllowEmpty = true,
        };
        var parser = new CommandParser(settings, commandSettings);
        parser.Parse(args);
        return settings;
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
