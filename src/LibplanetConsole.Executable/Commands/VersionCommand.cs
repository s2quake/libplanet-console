using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(VersionCommand))]
sealed class VersionCommand : VersionCommandBase
{
}
