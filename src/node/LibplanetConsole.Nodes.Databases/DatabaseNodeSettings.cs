using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Databases;

[ApplicationSettings]
internal sealed class DatabaseNodeSettings
{
    [CommandProperty(DefaultValue = "")]
    [CommandSummary("The absolute path(directory) of the database to store.")]
    public string DatabasePath { get; init; } = string.Empty;
}
