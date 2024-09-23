using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Consoles.Executable;
using LibplanetConsole.Consoles.Executable.EntryCommands;

var commands = new ICommand[]
{
    new InitializeCommand(),
    new StartCommand(),
    new RunCommand(),
    new KeyCommand(),
    new GenesisCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
