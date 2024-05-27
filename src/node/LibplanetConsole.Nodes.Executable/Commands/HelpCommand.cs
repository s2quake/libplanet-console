using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Nodes.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
internal sealed class HelpCommand : HelpCommandBase
{
}
