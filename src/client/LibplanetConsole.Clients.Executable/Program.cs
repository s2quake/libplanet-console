using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Clients.Executable;
using LibplanetConsole.Clients.Executable.EntryCommands;

var commands = new ICommand[]
{
    new RunCommand(),
    new KeyCommand(),
    new SchemaCommand(),
    new StartCommand(),
    new InitializeCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
