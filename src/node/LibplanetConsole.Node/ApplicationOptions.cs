using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using LibplanetConsole.Common.Converters;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Options;

namespace LibplanetConsole.Node;

[Options]
public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>
{
    public const string Position = "Application";
    public const int BlocksyncPortIncrement = 4;
    public const int ConsensusPortIncrement = 5;
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    public int Port { get; set; }

    [PrivateKey]
    [JsonConverter(typeof(PrivateKeyJsonConverter))]
    public PrivateKey PrivateKey { get; set; } = new PrivateKey();

    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    [JsonIgnore]
    public int ParentProcessId { get; set; }

    [EndPoint]
    [JsonConverter(typeof(EndPointJsonConverter))]
    public EndPoint? SeedEndPoint { get; set; }

    public string StorePath { get; set; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    [JsonIgnore]
    public bool NoREPL { get; set; }

    public string ActionProviderModulePath { get; set; } = string.Empty;

    public string ActionProviderType { get; set; } = string.Empty;

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
        return TryGetGenesis(out var g) == true ? g : CreateGenesis(PrivateKey);
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
