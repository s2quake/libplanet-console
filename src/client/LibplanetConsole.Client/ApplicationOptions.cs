using System.Text.Json.Serialization;
using LibplanetConsole.Common;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Options;

namespace LibplanetConsole.Client;

[Options]
public sealed class ApplicationOptions : OptionsBase<ApplicationOptions>, IApplicationOptions
{
    public const string Position = "Application";

    private PrivateKey? _privateKey;
    private EndPoint? _nodeEndPoint;

    public int Port { get; set; }

    [PrivateKey]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey
        => _privateKey ??= PrivateKeyUtility.ParseOrRandom(PrivateKey);

    [JsonIgnore]
    public int ParentProcessId { get; set; }

    [EndPoint]
    public string NodeEndPoint { get; set; } = string.Empty;

    public string Alias { get; set; } = string.Empty;

    EndPoint? IApplicationOptions.NodeEndPoint
        => _nodeEndPoint ??= EndPointUtility.ParseOrDefault(NodeEndPoint);

    public string LogPath { get; set; } = string.Empty;

    [JsonIgnore]
    public bool NoREPL { get; set; }
}
