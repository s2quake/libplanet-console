using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Frameworks;

namespace LibplanetConsole.Nodes.Examples;

[ApplicationSettings]
internal sealed class ExampleSettings
{
    public const string Example = nameof(Example);

    [CommandPropertySwitch("example")]
    [CommandSummary("This is switch for example. not used in real.")]
    [Category(Example)]
    public bool IsExample { get; init; }
}
