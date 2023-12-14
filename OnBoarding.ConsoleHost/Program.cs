using JSSoft.Library.Commands;
using JSSoft.Library.Terminals;
using OnBoarding.ConsoleHost;

try
{
    var options = ApplicationOptions.Parse(args);
    var @out = Console.Out;
    @out.WriteLine(TerminalStringBuilder.GetString("Welcome to jeesu world for OnBoarding.", TerminalColorType.BrightGreen));
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
