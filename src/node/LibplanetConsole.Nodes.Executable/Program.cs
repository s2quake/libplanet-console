using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Nodes.Executable;
using LibplanetConsole.Nodes.Executable.EntryCommands;

var commands = new ICommand[]
{
    new RunCommand(),
    new KeyCommand(),
    new GenesisCommand(),
    new SchemaCommand(),
    new StartCommand(),
    new InitializeCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
