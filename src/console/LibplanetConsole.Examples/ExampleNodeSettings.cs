using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Examples;

[ApplicationSettings]
internal sealed class ExampleNodeSettings
{
    [CommandPropertySwitch("node-example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsNodeExample { get; init; }
}
