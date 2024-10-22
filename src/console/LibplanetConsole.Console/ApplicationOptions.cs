using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using LibplanetConsole.Options;

namespace LibplanetConsole.Console;

[Options]
public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>, IApplicationOptions
{
    public const string Position = "Application";
    public const int SeedBlocksyncPortIncrement = 6;
    public const int SeedConsensusPortIncrement = 7;

    private byte[]? _genesis;

    public int Port { get; set; }

    [JsonIgnore]
    public NodeOptions[] Nodes { get; set; } = [];

    [JsonIgnore]
    public ClientOptions[] Clients { get; set; } = [];

    public string GenesisPath { get; set; } = string.Empty;

    [JsonIgnore]
    public string Genesis { get; set; } = string.Empty;

    public string LogPath { get; set; } = string.Empty;

    public bool NoProcess { get; set; }

    public bool Detach { get; set; }

    public bool NewWindow { get; set; }

    byte[] IApplicationOptions.Genesis
        => _genesis ??= TryGetGenesis(out var v) ? v : [];

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
