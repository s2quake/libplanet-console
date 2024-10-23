using System.Diagnostics.CodeAnalysis;
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
    private EndPoint? _seedEndPoint;
    private byte[]? _genesis;

    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey => ActualPrivateKey;

    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    byte[] IApplicationOptions.Genesis => _genesis ??= GetGenesis();

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

    public bool TryGetGenesis([MaybeNullWhen(false)] out byte[] genesis)
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

    public byte[] GetGenesis()
    {
        return TryGetGenesis(out var g) == true ? g : CreateGenesis(ActualPrivateKey);
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
}
