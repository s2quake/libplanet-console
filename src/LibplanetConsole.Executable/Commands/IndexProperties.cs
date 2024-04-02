using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

static class IndexProperties
{
    [CommandProperty('n', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the node. If omitted, the current node is used.")]
    public static int NodeIndex { get; set; }

    [CommandProperty('c', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the client. If omitted, the current client is used.")]
    public static int ClientIndex { get; set; }

    [CommandProperty('b', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the block. If omitted, the last block in the blockchain is used.")]
    public static long BlockIndex { get; set; }
}
