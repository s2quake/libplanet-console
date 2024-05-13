using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.ClientHost.Commands;

[Export(typeof(ICommand))]
[Export(typeof(HelpCommand))]
internal sealed class HelpCommand : HelpCommandBase
{
}
