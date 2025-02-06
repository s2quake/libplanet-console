using System.ComponentModel;
using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Node;
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
    private string? _appProtocolVersion;

    [PrivateKey]
    [Description("Specifies the private key of the genesis block.")]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey => ActualPrivateKey;

    [JsonIgnore]
    public NodeOptions[] Nodes { get; set; } = [];

    [JsonIgnore]
    public ClientOptions[] Clients { get; set; } = [];

    [Description("Specifies the path to the genesis block file.")]
    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    [Description("Specifies the path to the app protocol version file.")]
    public string AppProtocolVersionPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string AppProtocolVersion { get; set; } = string.Empty;

    [Description("Specifies the directory path to store the log files.")]
    public string LogPath { get; set; } = string.Empty;

    [Description("Specifies the directory path to store the aliases.")]
    public string AliasPath { get; set; } = string.Empty;

    [Description("If true, nodes and clienta will not be executed.")]
    public bool NoProcess { get; set; }

    [Description("If true, nodes and clients will be detached from the console after the process " +
                 "is executed.")]
    public bool Detach { get; set; }

    [Description("If true, nodes and clients will be executed in a new window.")]
    public bool NewWindow { get; set; }

    [Description("Specifies the path to the action provider module.")]
    public string ActionProviderModulePath { get; set; } = string.Empty;

    [Description("Specifies the type of the action provider.")]
    public string ActionProviderType { get; set; } = string.Empty;

    [Description("Specifies the port for the block sync of the seed service.")]
    public int BlocksyncPort { get; set; }

    [Description("Specifies the port for the consensus of the seed service.")]
    public int ConsensusPort { get; set; }

    Block IApplicationOptions.GenesisBlock => _genesisBlock ??= GetGenesisBlock();

    string IApplicationOptions.AppProtocolVersion
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

    private string GetAppProtocolVersion()
    {
        if (AppProtocolVersionPath != string.Empty)
        {
            var lines = File.ReadAllLines(AppProtocolVersionPath);
            return lines[0];
        }

        if (AppProtocolVersion != string.Empty)
        {
            return AppProtocolVersion;
        }

        throw new NotSupportedException("AppProtocolVersion is not set.");
    }
}
