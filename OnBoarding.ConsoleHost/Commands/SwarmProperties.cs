using System.ComponentModel.Composition;
using JSSoft.Library.Commands;
using OnBoarding.ConsoleHost.Serializations;

namespace OnBoarding.ConsoleHost.Commands;

static class SwarmProperties
{
    [CommandProperty("swarm-index", 's', InitValue = -1)]
    [CommandDescription("Indicates the specified swarm index. If omitted, uses the current index.")]
    public static int Index { get; set; }
}