using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Examples;

[ApplicationSettings]
internal sealed class ExampleSettings
{
    [CommandPropertySwitch("node-example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsNodeExample { get; init; }
}
