using System.Text.Json.Serialization;
using Libplanet.Net;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Options;

namespace LibplanetConsole.Console;

[Options]
public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>, IApplicationOptions
{
    public const string Position = "Application";
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    private PrivateKey? _privateKey;
    private Block? _genesisBlock;
    private AppProtocolVersion? _appProtocolVersion;

    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey => ActualPrivateKey;

    [JsonIgnore]
    public NodeOptions[] Nodes { get; set; } = [];

    [JsonIgnore]
    public ClientOptions[] Clients { get; set; } = [];

    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    public string AppProtocolVersionPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string AppProtocolVersion { get; set; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public bool NoProcess { get; set; }

    public bool Detach { get; set; }

    public bool NewWindow { get; set; }

    public string ActionProviderModulePath { get; set; } = string.Empty;

    public string ActionProviderType { get; set; } = string.Empty;

    public int BlocksyncPort { get; set; }

    public int ConsensusPort { get; set; }

    Block IApplicationOptions.GenesisBlock => _genesisBlock ??= GetGenesisBlock();

    AppProtocolVersion IApplicationOptions.AppProtocolVersion
        => _appProtocolVersion ??= GetAppProtocolVersion();

    ProcessOptions? IApplicationOptions.ProcessOptions
        => NoProcess ? null : new ProcessOptions { Detach = Detach, NewWindow = NewWindow, };

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

        throw new NotSupportedException("Genesis is not set.");
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

        throw new NotSupportedException("AppProtocolVersion is not set.");
    }
}
