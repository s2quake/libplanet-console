using JSSoft.Commands;

namespace LibplanetConsole.Nodes.Executable;

internal sealed class EntryCommandContext(params ICommand[] commands)
    : CommandContextBase(commands)
{
}
