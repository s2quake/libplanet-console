using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using Libplanet.Common;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Executable;

[ApplicationSettings(IsRequired = true)]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the EndPoint on which the node will run. " +
                    "If omitted, a random endpoint is used.")]
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
    [Category]
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
    [CommandSummary("Indicates the file path to load the genesis block.\n" +
                    "Mutually exclusive with '--genesis' option.")]
    [Path(ExistsType = PathExistsType.Exist, AllowEmpty = true)]
    public string GenesisPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandPropertyExclusion(nameof(GenesisPath))]
    [CommandSummary("Indicates a hexadecimal genesis string. If omitted, a random genesis block " +
                    "is used.\nMutually exclusive with '--genesis-path' option.")]
    [JsonIgnore]
    public string Genesis { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the file path to save logs.")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Indicates the file path to save logs for the library. " +
                    "If omitted, the library logs will be saved in the LogPath.")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    [DefaultValue("")]
    public string LibraryLogPath { get; init; } = string.Empty;

    [CommandPropertySwitch("no-repl")]
    [CommandSummary("If set, the node runs without a REPL.")]
    [JsonIgnore]
    public bool NoREPL { get; init; }

    [CommandPropertySwitch("single-node")]
    [CommandPropertyExclusion(nameof(SeedEndPoint))]
    [CommandSummary("If set, the node runs as a single node.\n" +
                    "Mutually exclusive with '--seed-endpoint' option.")]
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
            SeedEndPoint = GetSeedEndPoint(),
            StorePath = GetFullPath(StorePath),
            LogPath = GetFullPath(LogPath),
            LibraryLogPath = GetFullPath(LibraryLogPath),
            NoREPL = NoREPL,
            Components = components,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;

        AppEndPoint? GetSeedEndPoint()
        {
            if (SeedEndPoint != string.Empty)
            {
                return AppEndPoint.Parse(SeedEndPoint);
            }

            return IsSingleNode is true ? endPoint : null;
        }
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