using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.DataAnnotations;
using LibplanetConsole.Framework;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node.Executable;

[ApplicationSettings(IsRequired = true)]
internal sealed record class ApplicationSettings
{
    [CommandProperty]
    [CommandSummary("Indicates the port on which the node will run. " +
                    "If omitted, a random port is used.")]
    [NonNegative]
    public int Port { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the private key of the node. " +
                    "If omitted, a random private key is used.")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty("parent")]
    [CommandSummary("Reserved option used by libplanet-console.")]
    [JsonIgnore]
    [Category]
    public int ParentProcessId { get; init; }

    [CommandProperty]
    [CommandSummary("Indicates the EndPoint of the seed node to connect to.")]
    [CommandPropertyExclusion(nameof(IsSingleNode))]
    [EndPoint]
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
    [CommandSummary("Indicates the directory path to save logs.")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    [DefaultValue("")]
    public string LogPath { get; init; } = string.Empty;

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

    [CommandProperty("module-path")]
    [CommandSummary("Indicates the path or the name of the assembly that provides " +
                    "the IActionProvider.\n" +
                    "Requires the '--single-node' option to be set.")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    [DefaultValue("")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Indicates the type name of the IActionProvider.\n" +
                    "Requires the '--single-node' option to be set.")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    [DefaultValue("")]
    public string ActionProviderType { get; set; } = string.Empty;

    public ApplicationOptions ToOptions()
    {
        var port = Port == 0 ? PortUtility.NextPort() : Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var genesis = TryGetGenesis(out var g) == true ? g : CreateGenesis(privateKey);
        var actionProvider = ModuleLoader.LoadActionLoader(
            ActionProviderModulePath, ActionProviderType);
        return new ApplicationOptions(port, privateKey, genesis)
        {
            ParentProcessId = ParentProcessId,
            SeedEndPoint = GetSeedEndPoint(),
            StorePath = GetFullPath(StorePath),
            LogPath = GetFullPath(LogPath),
            NoREPL = NoREPL,
            ActionProvider = actionProvider,
        };

        static string GetFullPath(string path)
            => path != string.Empty ? Path.GetFullPath(path) : path;

        EndPoint? GetSeedEndPoint()
        {
            if (SeedEndPoint != string.Empty)
            {
                return Parse(SeedEndPoint);
            }

            return IsSingleNode is true ? GetLocalHost(port) : null;
        }
    }

    private static byte[] CreateGenesis(PrivateKey privateKey)
    {
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = privateKey,
            Validators = [privateKey.PublicKey],
            Timestamp = DateTimeOffset.UtcNow,
        };
        var genesisBlock = BlockUtility.CreateGenesisBlock(genesisOptions);
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
