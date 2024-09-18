﻿using JSSoft.Commands;
using JSSoft.Commands.Extensions;
using LibplanetConsole.Nodes.Executable;
using LibplanetConsole.Nodes.Executable.EntryCommands;

var commands = new ICommand[]
{
    new InitializeCommand(),
    new StartCommand(),
    new RunCommand(),
    new KeyCommand(),
    new GenesisCommand(),
    new SchemaCommand(),
};
var commandContext = new EntryCommandContext(commands);
await commandContext.ExecuteAsync(args);
