using System.ComponentModel;
using JSSoft.Commands;
using LibplanetConsole.Framework;

namespace LibplanetConsole.Node.Example;

[ApplicationSettings]
internal sealed class ExampleSettings
{
    public const string Example = nameof(Example);

    [CommandPropertySwitch("example")]
    [CommandSummary("This is switch for example. not used in real.")]
    [Category(Example)]
    public bool IsExample { get; init; }
}
