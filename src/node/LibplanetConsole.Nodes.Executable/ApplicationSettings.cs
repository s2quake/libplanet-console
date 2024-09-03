using System.Diagnostics.CodeAnalysis;
using JSSoft.Commands;
using Libplanet.Common;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[ApplicationSettings]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the Node Service will run. " +
                    "If omitted, host is 127.0.0.1 and port is set to random.")]
    [AppEndPoint]
    public string EndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    [AppPrivateKey]
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
    [AppEndPoint]
    public string NodeEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store data." +
                    "If omitted, the data is stored in memory.")]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyCondition(nameof(Genesis), "")]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyCondition(nameof(GenesisPath), "")]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The file path to store log.")]
    public string LogPath { get; init; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    public bool NoREPL { get; init; }

    public ApplicationOptions ToOptions(object[] components)
    {
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var privateKey = AppPrivateKey.ParseOrRandom(PrivateKey);
        var genesis = TryGetGenesis(out var g) == true ? g : CreateGenesis(privateKey);
        return new ApplicationOptions(endPoint, privateKey, genesis)
        {
            ParentProcessId = ParentProcessId,
            ManualStart = ManualStart,
            NodeEndPoint = AppEndPoint.ParseOrDefault(NodeEndPoint),
            StorePath = GetFullPath(StorePath),
            LogPath = GetFullPath(LogPath),
            NoREPL = NoREPL,
            Components = components,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;
    }

    private static byte[] CreateGenesis(AppPrivateKey privateKey)
    {
        var validatorKey = new AppPublicKey[]
        {
            privateKey.PublicKey,
        };
        var dateTimeOffset = DateTimeOffset.UtcNow;
        var genesisBlock = BlockUtility.CreateGenesisBlock(
            privateKey, validatorKey, dateTimeOffset);
        return BlockUtility.SerializeBlock(genesisBlock);
    }

    private bool TryGetGenesis([MaybeNullWhen(false)] out byte[] genesis)
    {
        if (GenesisPath != string.Empty)
        {
            var hex = File.ReadAllText(GenesisPath);
            genesis = ByteUtil.ParseHex(hex);
            return true;
        }

        if (Genesis != string.Empty)
        {
            genesis = ByteUtil.ParseHex(Genesis);
            return true;
        }

        genesis = null!;
        return false;
    }
}
