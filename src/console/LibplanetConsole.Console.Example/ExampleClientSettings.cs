using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Consoles.Examples;

[ApplicationSettings]
internal sealed class ExampleClientSettings
{
    [CommandPropertySwitch("client-example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsClientExample { get; init; }
}
