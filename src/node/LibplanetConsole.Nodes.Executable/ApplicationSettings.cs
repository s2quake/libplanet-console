using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[ApplicationSettings]
internal sealed record class ApplicationSettings
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

    [CommandPropertySwitch("manual-start", 'm')]
    [CommandSummary("If set, the node does not start automatically. " +
                    "Instead, it waits for the user to start it manually.")]
    public bool ManualStart { get; init; } = false;

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
    public string[] Validators { get; init; } = [];

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    public string LogPath { get; init; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    public bool NoREPL { get; init; }

    public static implicit operator ApplicationOptions(ApplicationSettings settings)
    {
        var endPoint = AppEndPoint.ParseOrNext(settings.EndPoint);
        var privateKey = AppPrivateKey.ParseOrRandom(settings.PrivateKey);
        return new ApplicationOptions(endPoint, privateKey)
        {
            ParentProcessId = settings.ParentProcessId,
            ManualStart = settings.ManualStart,
            NodeEndPoint = AppEndPoint.ParseOrDefault(settings.NodeEndPoint),
            Validators = settings.GetGenesisValidators(privateKey.PublicKey),
            StorePath = GetFullPath(settings.StorePath),
            LogPath = GetFullPath(settings.LogPath),
            NoREPL = settings.NoREPL,
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

    private AppPublicKey[] GetGenesisValidators(AppPublicKey publicKey) => Validators.Length switch
    {
        > 0 => [.. Validators.Select(AppPublicKey.Parse)],
        _ => [publicKey],
    };
}
