using JSSoft.Library.Commands;

namespace OnBoarding.ConsoleHost.Commands;

static class IndexProperties
{
    [CommandProperty('s', useName: true, InitValue = -1)]
    [CommandSummary("Indicates the specified swarm index.")]
    public static int SwarmIndex { get; set; }

    [CommandProperty('u', useName: true, InitValue = -1)]
    public static int UserIndex { get; set; }

    [CommandProperty('b', useName: true, InitValue = -1)]
    public static int BlockIndex { get; set; }
}
