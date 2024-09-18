using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Clients.Executable;
using LibplanetConsole.Clients.Executable.EntryCommands;

var commands = new ICommand[]
{
    new InitializeCommand(),
    new StartCommand(),
    new RunCommand(),
    new KeyCommand(),
    new SchemaCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
