using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Clients.Examples;

[ApplicationSettings]
internal sealed class ExampleClientSettings
{
    [CommandPropertySwitch("example")]
    [CommandSummary("This is switch for example. not used in real.")]
    public bool IsExample { get; init; }
}
