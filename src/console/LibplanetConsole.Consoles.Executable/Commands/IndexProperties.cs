using JSSoft.Commands;

namespace LibplanetConsole.Consoles.Executable.Commands;

internal static class IndexProperties
{
    [CommandProperty("node", 'n', InitValue = "")]
    [CommandSummary("Indicates the address of the node. If omitted, the current node is used.")]
    public static string Node { get; set; } = string.Empty;

    [CommandProperty("client", 'c', InitValue = "")]
    [CommandSummary("Indicates the address of the client. If omitted, the current client is used.")]
    public static string Client { get; set; } = string.Empty;
}
