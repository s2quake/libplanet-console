using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Clients.Executable;
using LibplanetConsole.Clients.Executable.EntryCommands;

var commands = new ICommand[]
{
    new RunCommand(),
    new KeyCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
