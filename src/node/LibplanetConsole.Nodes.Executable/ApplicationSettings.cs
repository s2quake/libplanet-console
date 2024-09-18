using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using Libplanet.Common;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Executable;

[ApplicationSettings(IsRequired = true)]
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
    [JsonIgnore]
    [Category("Hidden")]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the seed node to connect to.")]
    [CommandPropertyExclusion(nameof(IsSingleNode))]
    [AppEndPoint]
    public string SeedEndPoint { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("The directory path to store data." +
                    "If omitted, the data is stored in memory.")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    [DefaultValue("")]
    public string StorePath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(Genesis))]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [JsonIgnore]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the file path to save logs.")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the file path to save logs for the library." +
                    "If omitted, the library logs will be saved in the LogPath.")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    [DefaultValue("")]
    public string LibraryLogPath { get; init; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the REPL is not started.")]
    [JsonIgnore]
    public bool NoREPL { get; init; }

    [CommandPropertySwitch("single-node")]
    [CommandPropertyExclusion(nameof(SeedEndPoint))]
    [JsonIgnore]
    public bool IsSingleNode { get; set; }

    public ApplicationOptions ToOptions(object[] components)
    {
        var endPoint = AppEndPoint.ParseOrNext(EndPoint);
        var privateKey = AppPrivateKey.ParseOrRandom(PrivateKey);
        var genesis = TryGetGenesis(out var g) == true ? g : CreateGenesis(privateKey);
        return new ApplicationOptions(endPoint, privateKey, genesis)
        {
            ParentProcessId = ParentProcessId,
            SeedEndPoint = AppEndPoint.ParseOrDefault(SeedEndPoint),
            StorePath = GetFullPath(StorePath),
            LogPath = GetFullPath(LogPath),
            LibraryLogPath = GetFullPath(LibraryLogPath),
            NoREPL = NoREPL,
            IsSingleNode = IsSingleNode,
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
            var lines = File.ReadAllLines(GenesisPath);
            genesis = ByteUtil.ParseHex(lines[0]);
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
