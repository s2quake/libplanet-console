using System.ComponentModel;
using JSSoft.Commands;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Common.IO;
using LibplanetConsole.DataAnnotations;
using static LibplanetConsole.Common.EndPointUtility;

namespace LibplanetConsole.Node.Executable.EntryCommands;

[CommandSummary("Creates a repository to run the libplanet-node")]
internal sealed class InitializeCommand : CommandBase
{
    private static readonly Codec _codec = new();

    public InitializeCommand()
        : base("init")
    {
    }

    [CommandPropertyRequired]
    [CommandSummary("Specifies the directory path used to initialize a repository")]
    [Path(Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty)]
    public string RepositoryPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the private key of the node")]
    [PrivateKey]
    public string PrivateKey { get; init; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the port of the node")]
    [NonNegative]
    public int Port { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the directory path to store the block")]
    [Path(
        Type = PathType.Directory, ExistsType = PathExistsType.NotExistOrEmpty, AllowEmpty = true)]
    public string StorePath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the file path to save logs")]
    [Path(Type = PathType.Directory, AllowEmpty = true)]
    public string LogPath { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the EndPoint of the seed node to connect to")]
    [EndPoint]
    public string SeedEndPoint { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the file path of the genesis")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    public string GenesisPath { get; set; } = string.Empty;

    [CommandProperty("apv-path")]
    [CommandSummary("Specifies the file path of the app protocol version")]
    [Path(Type = PathType.File, AllowEmpty = true)]
    public string AppProtocolVersionPath { get; set; } = string.Empty;

    [CommandPropertySwitch("single-node")]
    [CommandSummary("If set, the repository is created in a format suitable for a single node")]
    public bool IsSingleNode { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the private key of the genesis block")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    [PrivateKey]
    [Category("Genesis")]
    public string GenesisKey { get; set; } = string.Empty;

    [CommandProperty("timestamp")]
    [CommandSummary("Specifies the timestamp of the genesis block")]
    [Category("Genesis")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    public DateTimeOffset DateTimeOffset { get; set; }

    [CommandPropertySwitch("quiet", 'q')]
    [CommandSummary("If set, the command does not output any information")]
    public bool Quiet { get; set; }

    [CommandProperty("module-path")]
    [CommandSummary("Specifies the path or the name of the assembly that provides " +
                    "the IActionProvider")]
    [Category("Genesis")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [CommandProperty("module-type")]
    [CommandSummary("Specifies the type name of the IActionProvider")]
    [CommandExample("--module-type 'LibplanetModule.SimpleActionProvider, LibplanetModule'")]
    [Category("Genesis")]
    public string ActionProviderType { get; set; } = string.Empty;

    [CommandProperty("apv-private-key")]
    [CommandSummary("Specifies the private key of the signer of the AppProtocolVersion")]
    [PrivateKey]
    [Category("AppProtocolVersion")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    public string APVPrivateKey { get; set; } = string.Empty;

    [CommandProperty("apv-version", InitValue = 1)]
    [CommandSummary("Specifies the version number of the AppProtocolVersion. Default is 1")]
    [Category("AppProtocolVersion")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    public int APVVersion { get; set; }

    [CommandProperty("apv-extra")]
    [CommandSummary("Specifies the extra data to be included in the AppProtocolVersion")]
    [Category("AppProtocolVersion")]
    [CommandPropertyDependency(nameof(IsSingleNode))]
    public string APVExtra { get; set; } = string.Empty;

    [CommandProperty]
    [CommandSummary("Specifies the port for the blocksync of the node")]
    public int BlocksyncPort { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the port for the consensus of the node")]
    public int ConsensusPort { get; set; }

    [CommandProperty]
    [CommandSummary("Specifies the alias of the node address")]
    public string Alias { get; set; } = string.Empty;

    protected override void OnExecute()
    {
        var outputPath = Path.GetFullPath(RepositoryPath);
        var port = Port == 0 ? PortUtility.NextPort() : Port;
        var privateKey = PrivateKeyUtility.ParseOrRandom(PrivateKey);
        var storePath = Path.Combine(outputPath, StorePath.Fallback("store"));
        var logPath = Path.Combine(outputPath, LogPath.Fallback("log"));
        var genesisPath = Path.Combine(outputPath, GenesisPath.Fallback("genesis"));
        var appProtocolVersionPath = Path.Combine(
            outputPath, AppProtocolVersionPath.Fallback("appProtocolVersion"));
        var blocksyncPort = BlocksyncPort is 0 ? PortUtility.NextPort() : BlocksyncPort;
        var consensusPort = ConsensusPort is 0 ? PortUtility.NextPort() : ConsensusPort;
        var repository = new Repository
        {
            Port = port,
            PrivateKey = privateKey,
            StorePath = storePath,
            LogPath = logPath,
            SeedEndPoint = ParseOrDefault(SeedEndPoint),
            GenesisPath = genesisPath,
            AppProtocolVersionPath = appProtocolVersionPath,
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
            BlocksyncPort = blocksyncPort,
            ConsensusPort = consensusPort,
            IsSingleNode = IsSingleNode,
            Alias = Alias,
        };
        dynamic info = repository.Save(outputPath);
        using var writer = new ConditionalTextWriter(Out)
        {
            Condition = Quiet is false,
        };

        if (IsSingleNode is true)
        {
            var genesisOptions = new GenesisOptions
            {
                GenesisKey = PrivateKeyUtility.ParseOrRandom(GenesisKey),
                Validators = [privateKey.PublicKey],
                Timestamp = DateTimeOffset != DateTimeOffset.MinValue
                    ? DateTimeOffset : DateTimeOffset.UtcNow,
                ActionProviderModulePath = ActionProviderModulePath,
                ActionProviderType = ActionProviderType,
            };

            var genesisBlock = BlockUtility.CreateGenesisBlock(genesisOptions);
            var genesis = BlockUtility.SerializeBlock(genesisBlock);
            var genesisString = ByteUtil.Hex(genesis);
            File.WriteAllBytes(genesisPath, genesis);
            info.GenesisArguments = new
            {
                GenesisKey = PrivateKeyUtility.ToString(genesisOptions.GenesisKey),
                Validators = genesisOptions.Validators.Select(
                    item => item.ToHex(compress: false)),
                genesisOptions.Timestamp,
                genesisOptions.ActionProviderModulePath,
                genesisOptions.ActionProviderType,
            };
            info.Genesis = genesisString;

            var apvPrivateKey = PrivateKeyUtility.ParseOrRandom(APVPrivateKey);
            var apvVersion = APVVersion;
            var apvExtra = APVExtra != string.Empty
                ? _codec.Decode(ByteUtil.ParseHex(APVExtra)) : null;
            var appProtocolVersion = AppProtocolVersion.Sign(apvPrivateKey, apvVersion, apvExtra);
            File.WriteAllLines(appProtocolVersionPath, [appProtocolVersion.Token]);

            info.AppProtocolVersionArguments = new
            {
                PrivateKey = PrivateKeyUtility.ToString(privateKey),
                Version = apvVersion,
                Extra = APVExtra,
            };
            info.AppProtocolVersion = appProtocolVersion.Token;
        }

        TextWriterExtensions.WriteLineAsJson(writer, info);
    }
}
