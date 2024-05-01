using System.ComponentModel.Composition;
using JSSoft.Commands;

namespace LibplanetConsole.Executable.Commands;

[Export(typeof(ICommand))]
[Export(typeof(VersionCommand))]
internal sealed class VersionCommand : VersionCommandBase
{
}
