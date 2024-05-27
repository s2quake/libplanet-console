using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Examples;

[ApplicationSettings]
internal sealed class ExampleNodeSettings
{
    [CommandPropertySwitch("example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsExample { get; init; }
}
