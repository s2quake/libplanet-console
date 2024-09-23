using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Console.Executable;
using LibplanetConsole.Console.Executable.EntryCommands;

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
