using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Common.Extensions;
using LibplanetConsole.Consoles.Executable;
using LibplanetConsole.Frameworks;

try
{
    var message = "Welcome to console for Libplanet.";
    var settings = ApplicationSettingsParser.Parse<ApplicationSettings>(args);
    var @out = Console.Out;
    @out.WriteColoredLine(message, TerminalColorType.BrightGreen);
    await using var application = new Application(settings);
    @out.WriteLine();
    await application.StartAsync();
    @out.WriteLine("\u001b0");
}
catch (CommandParsingException e)
{
    CommandUsageUtility.Print(Console.Out, e);
    Environment.Exit(1);
}
