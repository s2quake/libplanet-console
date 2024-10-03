using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
internal sealed class HelpCommand : HelpCommandBase
{
}
