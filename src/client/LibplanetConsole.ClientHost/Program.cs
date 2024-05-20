using JSSoft.Commands;
using LibplanetConsole.ClientHost;

try
{
    var options = ApplicationCommandOptions.Parse(args);
    var @out = Console.Out;
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
