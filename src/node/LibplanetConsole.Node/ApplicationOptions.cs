using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Options;

namespace LibplanetConsole.Node;

[Options]
public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>, IApplicationOptions
{
    public const string Position = "Application";
    public const int BlocksyncPortIncrement = 4;
    public const int ConsensusPortIncrement = 5;
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    private PrivateKey? _privateKey;
    private Uri? _hubUrl;
    private Block? _genesisBlock;
    private AppProtocolVersion? _appProtocolVersion;

    [PrivateKey]
    [Description("Specifies the private key to use.")]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey => ActualPrivateKey;

    [Description("Specifies the path to the genesis block file.")]
    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    [Description("Specifies the path to the app protocol version file.")]
    public string AppProtocolVersionPath { get; set; } = string.Empty;

    [JsonIgnore]
    [AppProtocolVersion]
    public string AppProtocolVersion { get; set; } = string.Empty;

    Block IApplicationOptions.GenesisBlock => _genesisBlock ??= GetGenesisBlock();

    AppProtocolVersion IApplicationOptions.AppProtocolVersion
        => _appProtocolVersion ??= GetAppProtocolVersion();

    [JsonIgnore]
    public int ParentProcessId { get; set; }

    [Uri(AllowEmpty = true)]
    [Description("Specifies the URL of the hub to connect to.")]
    public string HubUrl { get; set; } = string.Empty;

    Uri? IApplicationOptions.HubUrl
        => _hubUrl ??= UriUtility.ParseOrDefault(HubUrl);

    [Description("Specifies the directory path to store the store files.")]
    public string StorePath { get; set; } = string.Empty;

    [Description("Specifies the directory path to store the log files.")]
    public string LogPath { get; set; } = string.Empty;

    [JsonIgnore]
    public bool NoREPL { get; set; }

    [Description("Specifies the port to bind to.")]
    public bool IsSingleNode { get; set; }

    [Description("Specifies the path to the action provider module.")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [Description("Specifies the type of the action provider.")]
    public string ActionProviderType { get; set; } = string.Empty;

    [Description("Specifies the port for the block sync of the swarm.")]
    public int BlocksyncPort { get; set; }

    [Description("Specifies the port for the consensus of the swarm.")]
    public int ConsensusPort { get; set; }

    private PrivateKey ActualPrivateKey
        => _privateKey ??= PrivateKeyUtility.ParseOrRandom(PrivateKey);

    private Block GetGenesisBlock()
    {
        if (GenesisPath != string.Empty)
        {
            return BlockUtility.LoadGenesisBlock(GenesisPath);
        }

        if (Genesis != string.Empty)
        {
            return BlockUtility.DeserializeBlock(ByteUtil.ParseHex(Genesis));
        }

        var privateKey = ActualPrivateKey;
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = privateKey,
            Validators = [privateKey.PublicKey],
            Timestamp = DateTimeOffset.UtcNow,
            ActionProviderModulePath = ActionProviderModulePath,
            ActionProviderType = ActionProviderType,
        };

        return BlockUtility.CreateGenesisBlock(genesisOptions);
    }

    private AppProtocolVersion GetAppProtocolVersion()
    {
        if (AppProtocolVersionPath != string.Empty)
        {
            var lines = File.ReadAllLines(AppProtocolVersionPath);
            return Libplanet.Net.AppProtocolVersion.FromToken(lines[0]);
        }

        if (AppProtocolVersion != string.Empty)
        {
            return Libplanet.Net.AppProtocolVersion.FromToken(AppProtocolVersion);
        }

        var privateKey = new PrivateKey();
        return Libplanet.Net.AppProtocolVersion.Sign(privateKey, 1);
    }
}
