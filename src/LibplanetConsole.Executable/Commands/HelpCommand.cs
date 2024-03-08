using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
sealed class HelpCommand : HelpCommandBase
{
}
