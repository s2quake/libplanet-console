using JSSoft.Commands;

namespace LibplanetConsole.Clients.Executable;

internal sealed class EntryCommandContext(params ICommand[] commands)
    : CommandContextBase(commands)
{
}
