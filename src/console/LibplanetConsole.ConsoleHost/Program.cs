using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.ConsoleHost;

try
{
    var message = "Welcome to console for Libplanet.";
    var options = ApplicationCommandOptions.Parse(args);
    var @out = Console.Out;
    @out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    await using var application = new Application(options);
    @out.WriteLine();
    await application.StartAsync();
    @out.WriteLine("\u001b0");
}
catch (CommandParsingException e)
{
    CommandUsageUtility.Print(Console.Out, e);
    Environment.Exit(1);
}
