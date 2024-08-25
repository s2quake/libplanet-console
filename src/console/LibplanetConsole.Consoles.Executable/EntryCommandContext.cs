using JSSoft.Commands;

namespace LibplanetConsole.Consoles.Executable;

internal sealed class EntryCommandContext(params ICommand[] commands)
    : CommandContextBase(commands)
{
}
