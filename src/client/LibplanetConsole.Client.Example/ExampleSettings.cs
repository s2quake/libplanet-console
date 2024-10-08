using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Client.Example;

[ApplicationSettings]
internal sealed class ExampleSettings
{
    [CommandPropertySwitch("example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsExample { get; init; }
}
