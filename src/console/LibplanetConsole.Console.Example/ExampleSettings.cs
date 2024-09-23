using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Console.Example;

[ApplicationSettings]
internal sealed class ExampleSettings
{
    [CommandPropertySwitch("node-example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsNodeExample { get; init; }
}
