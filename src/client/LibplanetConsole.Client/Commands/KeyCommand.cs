using System.ComponentModel.Composition;
using JSSoft.Commands;
using LibplanetConsole.Common.Commands;

namespace LibplanetConsole.Client.Commands;

[Export(typeof(ICommand))]
internal sealed class KeyCommand : KeyCommandBase
{
}
