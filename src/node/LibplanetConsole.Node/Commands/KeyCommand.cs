using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Commands;

namespace LibplanetConsole.Node.Commands;

[Export(typeof(ICommand))]
internal sealed class KeyCommand : KeyCommandBase
{
}
