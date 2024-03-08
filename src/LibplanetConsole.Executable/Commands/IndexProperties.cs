using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

static class IndexProperties
{
    [CommandProperty('s', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the user. If omitted, the current swarm is used.")]
    public static int SwarmIndex { get; set; }

    [CommandProperty('u', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the user. If omitted, the current user is used.")]
    public static int UserIndex { get; set; }

    [CommandProperty('b', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the index of the block. If omitted, the last block in the blockchain is used.")]
    public static long BlockIndex { get; set; }
}
