using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Node.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
internal sealed class HelpCommand : HelpCommandBase
{
}
