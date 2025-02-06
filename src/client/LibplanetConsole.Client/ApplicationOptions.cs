using System.ComponentModel;
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
    private Uri? _hubUrl;

    [Description("Specifies the address to bind to.")]
    public int Port { get; set; }

    [PrivateKey]
    [Description("Specifies the private key to use.")]
    public string PrivateKey { get; set; } = string.Empty;

    PrivateKey IApplicationOptions.PrivateKey
        => _privateKey ??= PrivateKeyUtility.ParseOrRandom(PrivateKey);

    [JsonIgnore]
    public int ParentProcessId { get; set; }

    [Uri(AllowEmpty = true)]
    [Description("Specifies the URL of the hub to connect to.")]
    public string HubUrl { get; set; } = string.Empty;

    Uri? IApplicationOptions.HubUrl
        => _hubUrl ??= new Uri(HubUrl);

    [Description("Specifies the directory path to store the log files.")]
    public string LogPath { get; set; } = string.Empty;

    [JsonIgnore]
    public bool NoREPL { get; set; }
}
