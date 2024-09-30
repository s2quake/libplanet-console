using System.ComponentModel;
using System.Text.Json.Serialization;
using JSSoft.Commands;
using LibplanetConsole.Common.DataAnnotations;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Explorer;

[ApplicationSettings]
internal sealed class ExplorerSettings
{
    public const string Explorer = nameof(Explorer);

    [CommandPropertySwitch("explorer")]
    [CommandSummary("")]
    [JsonPropertyName("isEnabled")]
    [Category(Explorer)]
    public bool IsExplorerEnabled { get; init; }

    [CommandProperty("explorer-end-point", DefaultValue = "")]
    [CommandSummary("")]
    [CommandPropertyDependency(nameof(IsExplorerEnabled))]
    [JsonPropertyName("endPoint")]
    [EndPoint]
    [Category(Explorer)]
    public string ExplorerEndPoint { get; init; } = string.Empty;
}
