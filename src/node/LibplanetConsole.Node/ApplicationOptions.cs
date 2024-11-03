using System.Text.Json.Serialization;
using Libplanet.Net;
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
    private EndPoint? _seedEndPoint;
    private byte[]? _genesis;
    private AppProtocolVersion? _appProtocolVersion;

    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey => ActualPrivateKey;

    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    public string AppProtocolVersionPath { get; set; } = string.Empty;

    [JsonIgnore]
    [AppProtocolVersion]
    public string AppProtocolVersion { get; set; } = string.Empty;

    byte[] IApplicationOptions.Genesis => _genesis ??= GetGenesis();

    AppProtocolVersion IApplicationOptions.AppProtocolVersion
        => _appProtocolVersion ??= GetAppProtocolVersion();

    [JsonIgnore]
    public int ParentProcessId { get; set; }

    [EndPoint]
    public string SeedEndPoint { get; set; } = string.Empty;

    EndPoint? IApplicationOptions.SeedEndPoint
        => _seedEndPoint ??= EndPointUtility.ParseOrDefault(SeedEndPoint);

    public string StorePath { get; set; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    [JsonIgnore]
    public bool NoREPL { get; set; }

    public string ActionProviderModulePath { get; set; } = string.Empty;

    public string ActionProviderType { get; set; } = string.Empty;

    private PrivateKey ActualPrivateKey
        => _privateKey ??= PrivateKeyUtility.ParseOrRandom(PrivateKey);

    private byte[] GetGenesis()
    {
        if (GenesisPath != string.Empty)
        {
            var lines = File.ReadAllLines(GenesisPath);
            return ByteUtil.ParseHex(lines[0]);
        }

        if (Genesis != string.Empty)
        {
            return ByteUtil.ParseHex(Genesis);
        }

        var privateKey = ActualPrivateKey;
        var genesisOptions = new GenesisOptions
        {
            GenesisKey = privateKey,
            Validators = [privateKey.PublicKey],
            Timestamp = DateTimeOffset.UtcNow,
        };
        var genesisBlock = BlockUtility.CreateGenesisBlock(genesisOptions);
        return BlockUtility.SerializeBlock(genesisBlock);
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
