using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
internal sealed class HelpCommand : HelpCommandBase
{
}
