using JSSoft.Commands;
using JSSoft.Terminals;
using LibplanetConsole.Executable;

try
{
    var options = ApplicationOptions.Parse(args);
    var @out = Console.Out;
    @out.WriteLine(TerminalStringBuilder.GetString("Welcome to console for Libplanet.", TerminalColorType.BrightGreen));
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
