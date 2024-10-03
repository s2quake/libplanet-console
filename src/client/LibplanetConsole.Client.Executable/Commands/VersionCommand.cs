using JSSoft.Commands;
using LibplanetConsole.Common;

namespace LibplanetConsole.Client.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(VersionCommand))]
internal sealed class VersionCommand : VersionCommandBase
{
}
