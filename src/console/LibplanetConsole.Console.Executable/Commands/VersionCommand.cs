using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Console.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(VersionCommand))]
internal sealed class VersionCommand : VersionCommandBase
{
}
